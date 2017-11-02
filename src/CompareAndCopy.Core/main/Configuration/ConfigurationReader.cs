using CompareAndCopy.Core.Compare;
using CompareAndCopy.Core.Copy;
using CompareAndCopy.Core.Filters;
using CompareAndCopy.Core.Locking;
using CompareAndCopy.Core.PathResolving;
using CompareAndCopy.Core.State;
using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ByteSizeLib;
using CompareAndCopy.Model.Configuration;
using CompareAndCopy.Model.Filtering;
using CompareAndCopy.Model.Actions;
using CompareAndCopy.Model.State;
using CompareAndCopy.Core.Configuration.Migration;

namespace CompareAndCopy.Core.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        const string s_ConfigurationSchema = "CompareAndCopy.Core.Configuration.ConfigurationSchema_Strict.xsd";

        readonly IConfigurationMigrator m_Migrator;

        
        public ConfigurationReader() : this(new ConfigurationMigrator())
        {
        }

        public ConfigurationReader(IConfigurationMigrator migrator)
        {
            m_Migrator = migrator ?? throw new ArgumentNullException("migrator");
        }


        /// <summary>
        /// Reads sync configuration from the specified file
        /// </summary>
        public ISyncConfiguration ReadConfiguration(string fileName)
        {
            var configFile = XDocument.Load(fileName);
            var pathResolver = new PathResolver(Path.GetDirectoryName(fileName));
            configFile = m_Migrator.UpgradeConfigurationFile(configFile);

            return ReadConfiguration(configFile, pathResolver);
        }

        public ISyncConfiguration ReadConfiguration(XDocument document, IPathResolver pathResolver)
        {
            //upgrade configuration file
            document = m_Migrator.UpgradeConfigurationFile(document);

            //validate configuration file
            document.Validate(
                GetStrictConfigutationSchema(), 
                (s, e) => throw new ConfigurationException("Invalid configuration file: " + e.Message));

            ISyncConfiguration configuration = new SyncConfiguration();
            try
            {
                ReadSyncConfiguration(document, configuration, pathResolver);
            }
            catch (ArgumentException ex)
            {
                throw new ConfigurationException("Error reading configuration", ex);
            }

            return configuration;
        }
        
        
        void ReadSyncConfiguration(XDocument configFile, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            foreach (var element in configFile.Root.Elements())
            {
                if (element.Name == XmlNames.Left)
                {
                    configuration.Left = ReadSyncFolderDefinition(element, pathResolver);
                }
                else if (element.Name == XmlNames.Right)
                {
                    configuration.Right = ReadSyncFolderDefinition(element, pathResolver);
                }
                else if (element.Name == XmlNames.Filter)
                {
                    var filter = ReadFilter(element);
                    configuration.AddFilter(filter);
                }
                else if (element.Name == XmlNames.Include)
                {
                    ReadInclude(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.TransferLocation)
                {
                    configuration.AddTransferLocation(ReadTransferLocation(element, pathResolver));
                }
                else if (element.Name == XmlNames.Actions)
                {
                    ReadActionList(element, configuration, pathResolver);
                }
                else
                {
                    throw new ConfigurationException("Unknown element " + element.Name.LocalName + " in Configuration");
                }
            }
        }

        ISyncFolderDefinition ReadSyncFolderDefinition(XElement xmlNode, IPathResolver pathResolver)
        {
            var name = xmlNode.RequireAttributeValue(XmlAttributeNames.Name);
            var rootPath = pathResolver.GetAbsolutePath(xmlNode.Attribute(XmlAttributeNames.RootPath).Value);

            if (xmlNode.Name == XmlNames.Left)
            {
                return new SyncFolderDefinition(name, rootPath, SyncFolder.Left);
            }
            else if (xmlNode.Name == XmlNames.Right)
            {
                return new SyncFolderDefinition(name, rootPath, SyncFolder.Right);
            }
            else
            {
                throw new ConfigurationException(String.Format("Cannot parse xml element '{0} as SyncFolderDefinition'",
                                                               xmlNode.Name.LocalName));
            }
        }

        void ReadInclude(XElement includeElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var includePath = pathResolver.GetAbsolutePath(includeElement.RequireAttributeValue(XmlAttributeNames.Path));

            if (!File.Exists(includePath))
            {
                throw new ConfigurationException(String.Format("Included configuration file '{0}' not found", includePath));
            }

            var configurationDocument = XDocument.Load(includePath);

            configurationDocument = m_Migrator.UpgradeConfigurationFile(configurationDocument);

            var includePathResolver = new PathResolver(Path.GetDirectoryName(includePath));
            ReadSyncConfiguration(configurationDocument, configuration, includePathResolver);

        }

        IFilter ReadFilter(XElement filterNode)
        {
            var name = filterNode.RequireAttributeValue(XmlAttributeNames.Name);
            if (filterNode.Elements().Count() != 1)
            {
                throw new ConfigurationException($"Invalid configuration. Expected a single root expression in filter '{name}'");
            }

            var rootExpression = ReadFilterExpression(filterNode.Elements().First());

            return new Filter(name, rootExpression);
        }

        IFilterExpression ReadFilterExpression(XElement filterExpressionNode)
        {
            if (filterExpressionNode.Name == XmlNames.Regex)
            {
                var pattern = filterExpressionNode.RequireAttributeValue(XmlAttributeNames.Pattern);
                return new RegexFilterExpression(pattern);
            }
            else if (filterExpressionNode.Name == XmlNames.CompareState)
            {
                var compareState = ParseCompareState(filterExpressionNode.RequireAttributeValue(XmlAttributeNames.Value));
                return new CompareStateFilterExpression(compareState);
            }
            else if (filterExpressionNode.Name == XmlNames.TransferState)
            {
                var transferState = ParseTransferState(filterExpressionNode.RequireAttributeValue(XmlAttributeNames.Value));
                return new TransferStateFilterExpression(transferState);
            }
            else if (filterExpressionNode.Name == XmlNames.MicroscopeQuery)
            {
                var query = filterExpressionNode.RequireAttributeValue(XmlAttributeNames.Query);
                return new MicroscopeFilterExpression(query);
            }
            else if (filterExpressionNode.Name == XmlNames.And)
            {
                var expressions = filterExpressionNode.Elements().Select(ReadFilterExpression);
                return new AndFilterExpression(expressions);
            }
            else if (filterExpressionNode.Name == XmlNames.Or)
            {
                var expressions = filterExpressionNode.Elements().Select(ReadFilterExpression);
                return new OrFilterExpression(expressions);
            }
            else if (filterExpressionNode.Name == XmlNames.Not)
            {
                if (filterExpressionNode.Elements().Count() != 1)
                {
                    throw new ConfigurationException("Invalid configuration, expected a single expression in 'not' expression");
                }

                var negatedExpression = ReadFilterExpression(filterExpressionNode.Elements().First());
                return new NotFilterExpression(negatedExpression);
            }
            else
            {
                throw new ConfigurationException($"Unknown filter-expression '{filterExpressionNode.Name.LocalName}'");
            }
        }


        void ReadActionList(XElement actionListElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            foreach (var actionElement in actionListElement.Elements())
            {
                ReadAction(actionElement, configuration, pathResolver);
            }
        }

        void ReadAction(XElement element, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
			if (element.Name == XmlNames.Compare)
			{
				configuration.AddAction(ReadCompareAction(element, configuration));
			}
			else if (element.Name == XmlNames.Export)
			{
				configuration.AddAction(ReadExportAction(element, configuration, pathResolver));
			}
			else if (element.Name == XmlNames.Import)
			{
				configuration.AddAction(ReadImportAction(element, configuration, pathResolver));
			}
			else if (element.Name == XmlNames.ReadSyncState)
			{
				configuration.AddAction(ReadReadSyncStateAction(element, configuration, pathResolver));
			}
			else if (element.Name == XmlNames.WriteSyncState)
			{
				configuration.AddAction(ReadWriteSyncStateAction(element, configuration, pathResolver));
			}
			else if (element.Name == XmlNames.ApplyFilter)
			{
				configuration.AddAction(ReadApplyFilerAction(element, configuration));
			}
			else if (element.Name == XmlNames.Copy)
			{
				configuration.AddAction(ReadCopyAction(element, configuration));
			}
			else if (element.Name == XmlNames.AcquireLock)
			{
				configuration.AddAction(ReadAquireLockAction(element, configuration, pathResolver));
			}
			else if (element.Name == XmlNames.ReleaseLock)
			{
				configuration.AddAction(ReadReleaseLockAction(element, configuration, pathResolver));
			}
			else if (element.Name == XmlNames.Sleep)
			{
				configuration.AddAction(ReadSleepAction(element, configuration));
			}
			else if (element.Name == XmlNames.RunSyncJob)
			{
				configuration.AddAction(ReadRunSyncJobAction(element, configuration, pathResolver));
			}
            else if (element.Name == XmlNames.UpdateTransferState)
            {
                configuration.AddAction(ReadUpdateTransferStateAction(element, configuration, pathResolver));
            }
            else if (element.Name == XmlNames.ExportDirectory)
            {
                configuration.AddAction(ReadExportDirectoryAction(element, configuration, pathResolver));
            }
            else if(element.Name == XmlNames.TouchFile)
            {
                configuration.AddAction(ReadTouchFileAction(element, configuration, pathResolver));
            }
            else
			{
				throw new ConfigurationException("Unknown element " + element.Name.LocalName + " in Configuration");
			}
        }

        IAction ReadCompareAction(XElement actionElement, ISyncConfiguration configuration)
        {
            var enabled = ReadActionEnabled(actionElement);
            var filterName = ReadActionInputFilterName(actionElement);

			var timeStampMarginElement = actionElement.Element(XmlNames.TimeStampMargin);
			var timeStampMargin = (timeStampMarginElement == null)
				? TimeSpan.FromSeconds(0)
				: ReadTimeSpan(timeStampMarginElement);
							
            return new CompareAction(enabled, filterName, configuration, timeStampMargin);
        }

        IAction ReadExportAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);
            var inputFilterName = ReadActionInputFilterName(actionElement);
            var syncFolder = ReadActionSyncFolder(actionElement);

            var action = new ExportAction(enabled, configuration, inputFilterName, syncFolder);
            ApplyCommonImportExportActionProperties(actionElement, action, configuration, pathResolver);
            return action;
        }

        IAction ReadImportAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);
            var inputFilterName = ReadActionInputFilterName(actionElement);
            var syncFolder = ReadActionSyncFolder(actionElement);

            var action = new ImportAction(enabled, configuration, inputFilterName, syncFolder);
            ApplyCommonImportExportActionProperties(actionElement, action, configuration, pathResolver);
            return action;
        }

        IAction ReadCopyAction(XElement actionElement, ISyncConfiguration configuration)
        {
            var enabled = ReadActionEnabled(actionElement);
            var inputFilterName = ReadActionInputFilterName(actionElement);
            var syncFolder = ReadActionSyncFolder(actionElement);

            var actionInstance = new CopyAction(enabled, configuration, inputFilterName, syncFolder);
            return actionInstance;
        }

        IAction ReadExportDirectoryAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);            
            // SyncFolder and input filter not relevant for this action, so just use default values
            var action = new ExportDirectoryAction(enabled, configuration, default(string), default(SyncFolder))
            {
                SourcePath = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.Path)),
                DeleteSourceFiles = ParseBool(actionElement.Attribute(XmlAttributeNames.DeleteSourceFiles)?.Value ?? "false")
            };
            ApplyCommonImportExportActionProperties(actionElement, action, configuration, pathResolver);
            return action;
        }

        IAction ReadTouchFileAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);
            var path = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.Path));
            var action = new TouchFileAction(enabled, configuration, path);
            return action;
        }

        SyncFolder ReadActionSyncFolder(XElement actionElement)
        {
            return ParseSource(actionElement.RequireAttributeValue(XmlAttributeNames.SyncFolder));
        }

        void ApplyCommonImportExportActionProperties(XElement actionElement, ImportExportAction actionInstance, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            actionInstance.TransferLocationName = actionElement.RequireAttributeValue(XmlAttributeNames.TransferLocationName);

            var subPathAttribute = actionElement.Attribute(XmlAttributeNames.TransferLocationSubPath);
            actionInstance.TransferLocationSubPath = subPathAttribute?.Value ?? String.Empty;

            var exclusiveWriteAccessAttribute = actionElement.Attribute(XmlAttributeNames.AssumeExclusiveWriteAccess);
            actionInstance.AssumeExclusiveWriteAccess = exclusiveWriteAccessAttribute != null && ParseBool(exclusiveWriteAccessAttribute.Value);
        }

        bool ParseBool(string value)
        {
            if (value == "0")
            {
                return false;
            }
            else if (value == "1")
            {
                return true;
            }
            else
            {
                return bool.Parse(value);
            }
        }

        ByteSize ReadByteSize(XElement byteSizeElement)
        {
            var teraByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.TeraByte);
            var gigaByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.GigaByte);
            var megaByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.MegaByte);
            var kiloByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.KiloByte);
            var bytes = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Byte);

            return ByteSize.FromBytes(bytes)
                .AddKiloBytes(kiloByte)
                .AddMegaBytes(megaByte)
                .AddGigaBytes(gigaByte)
                .AddTeraBytes(teraByte);
        }

        IAction ReadReadSyncStateAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);
            var fileName = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.FileName));

            return new ReadSyncStateAction(enabled, configuration, fileName);
        }

        IAction ReadWriteSyncStateAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);
            var inputFilterName = ReadActionInputFilterName(actionElement);
            var fileName = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.FileName));

            return new WriteSyncStateAction(enabled, configuration, inputFilterName, fileName);
        }

        IAction ReadApplyFilerAction(XElement actionElement, ISyncConfiguration configuration)
        {
            var enabled = ReadActionEnabled(actionElement);
            var inputFilterName = ReadActionInputFilterName(actionElement);

            return new ApplyFilterAction(enabled, configuration, inputFilterName);
        }

        bool ReadActionEnabled(XElement actionElement)
        {
            var value = actionElement.RequireAttributeValue(XmlAttributeNames.Enable).Trim();
            return ParseBool(value);
        }

        string ReadActionInputFilterName(XElement actionElement)
        {
            var inputFilterAttribute = actionElement.Attribute(XmlAttributeNames.InputFilter);
            if (inputFilterAttribute != null)
            {
                var value = inputFilterAttribute.Value;
                if(String.IsNullOrEmpty(value))
                {
                    throw new ConfigurationException(
                        $"The value for attribute {XmlAttributeNames.InputFilter} must not be empty (action {actionElement.Name.LocalName})"
                    );
                }

                return value;
            }
            else
            {
                return null;
            }
        }

        IAction ReadAquireLockAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);
            var lockFile = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.LockFile));

            TimeSpan? timeout = null;
            if (actionElement.Element(XmlNames.Timeout) != null)
            {
                timeout = ReadTimeSpan(actionElement.Element(XmlNames.Timeout));
            }

            var action = new AcquireLockAction(enabled, configuration, lockFile, timeout);
            return action;
        }

        IAction ReadReleaseLockAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);
            var lockFile = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.LockFile));

            return new ReleaseLockAction(enabled, configuration, lockFile);
        }

        IAction ReadSleepAction(XElement actionElement, ISyncConfiguration configuration)
        {
            var enabled = ReadActionEnabled(actionElement);
            var timeout = ReadTimeSpan(actionElement.Element(XmlNames.Timeout));

            return new SleepAction(enabled, configuration, timeout);
        }

		IAction ReadRunSyncJobAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResovler)
		{
			var enabled = ReadActionEnabled(actionElement);
			var path = pathResovler.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.Path));

			var action = new RunSyncJobAction(enabled, configuration, null, path);
			return action;
        }

        IAction ReadUpdateTransferStateAction(XElement actionElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var enabled = ReadActionEnabled(actionElement);

            var transferLocationPaths = actionElement.Elements(XmlNames.TransferLocation).Select(ReadTransferLocationReference);
            
            var interimLocations = actionElement.Elements(XmlNames.InterimLocation)
                .Select(xml => xml.RequireAttributeValue(XmlAttributeNames.Path))
                .Select(pathResolver.GetAbsolutePath);

            return new UpdateTransferStateAction(enabled, configuration, null, transferLocationPaths, interimLocations);
        }

        TransferLocation ReadTransferLocation(XElement transferLocationElement, IPathResolver pathResolver)
        {
            ByteSize? maximumSize = null;
            if (transferLocationElement.Element(XmlNames.MaximumSize) != null)
            {
                maximumSize = ReadByteSize(transferLocationElement.Element(XmlNames.MaximumSize));
            }

            return new TransferLocation(transferLocationElement.RequireAttributeValue(XmlAttributeNames.Name),
                pathResolver.GetAbsolutePath(transferLocationElement.RequireAttributeValue(XmlAttributeNames.Path)),
                maximumSize);
        }

        TransferLocationReference ReadTransferLocationReference(XElement element)
        {
            return new TransferLocationReference(element.RequireAttributeValue(XmlAttributeNames.TransferLocationName),
                element.RequireAttributeValue(XmlAttributeNames.TransferLocationSubPath));
        }

        CompareState ParseCompareState(string value)
        {
            if (!Enum.TryParse(value, true, out CompareState state))
            {
                throw new ArgumentException("Could not parse '" + value + "' as FileState");
            }
            return state;
        }

        TransferDirection ParseTransferState(string value)
        {
            if (!Enum.TryParse(value, true, out TransferDirection state))
            {
                throw new ArgumentException("Could not parse '" + value + "' as TransferState");
            }
            return state;
        }

        SyncFolder ParseSource(string value)
        {
            if (!Enum.TryParse(value, true, out SyncFolder source))
            {
                throw new ArgumentException("Could not parse '" + value + "' as Source");
            }
            return source;
        }

        TimeSpan ReadTimeSpan(XElement timespanElement)
        {
            var hours = (int)timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Hours);
            var minutes = (int)timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Minutes);
            var seconds = (int)timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Seconds);
            var milliSeconds = (int)timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.MilliSeconds);

            return new TimeSpan(0, hours, minutes, seconds, milliSeconds);
        }

        XmlSchemaSet GetStrictConfigutationSchema()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(s_ConfigurationSchema))
            {
                var schemaSet = new XmlSchemaSet();
                schemaSet.Add(null, XmlReader.Create(stream));

                return schemaSet;
            }
        }
    }
}
