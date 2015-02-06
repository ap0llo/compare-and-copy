using ServerSync.Core.Compare;
using ServerSync.Core.Copy;
using ServerSync.Core.Filters;
using ServerSync.Core.Locking;
using ServerSync.Core.PathResolving;
using ServerSync.Core.State;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ServerSync.Model.Configuration;
using ServerSync.Model.Filtering;
using ServerSync.Model.Actions;
using ServerSync.Model.State;
using ServerSync.Core.Configuration.Migration;

namespace ServerSync.Core.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {

        #region Fields

        readonly IConfigurationMigrator m_Migrator;

        #endregion


        #region Constants

        const string s_ConfigurationSchema = "ServerSync.Core.Configuration.ConfigurationSchema.xsd";

        #endregion


        #region Constructor

        public ConfigurationReader(IConfigurationMigrator migrator)
        {
            if(migrator == null)
            {
                throw new ArgumentNullException("migrator");
            }

            this.m_Migrator = migrator;
        }

        #endregion


        #region Public  Methods

        /// <summary>
        /// Reads sync configuration from the specified file
        /// </summary>
        public ISyncConfiguration ReadConfiguration(string fileName)
        {
            XDocument configFile = XDocument.Load(fileName);
            var pathResolver = new PathResolver(Path.GetDirectoryName(fileName));
            configFile = m_Migrator.UpgradeConfigurationFile(configFile);

            return ReadConfiguration(configFile, pathResolver);
            
        }


        public ISyncConfiguration ReadConfiguration(XDocument document, IPathResolver pathResolver)
        {
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

        #endregion Public Methods


        #region Private Implementation

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
                else if (element.Name == XmlNames.TimeStampMargin)
                {
                    configuration.TimeStampMargin = ReadTimeSpan(element);
                }
                else if (element.Name == XmlNames.Filter)
                {
                    var filter = ReadFilter(element);
                    configuration.AddFilter(filter);
                }
                else if(element.Name == XmlNames.Include)
                {
                    ReadInclude(element, configuration, pathResolver);
                }
                else if(element.Name == XmlNames.TransferLocation)
                {
                    configuration.AddTransferLocation(ReadTransferLocation(element, pathResolver));
                }
                else if(element.Name == XmlNames.Actions)
                {
                    ReadActionList(element, configuration, pathResolver);
                }
                else
                {
                    throw new ConfigurationException("Unknown element " + element.Name.LocalName + " in Configuration");
                }
            }
        }

        #region SyncFolderDefinition

        ISyncFolderDefinition ReadSyncFolderDefinition(XElement xmlNode, IPathResolver pathResolver)
        {
            var name = xmlNode.RequireAttributeValue(XmlAttributeNames.Name);
            var rootPath = pathResolver.GetAbsolutePath(xmlNode.Attribute(XmlAttributeNames.RootPath).Value);

            if(xmlNode.Name == XmlNames.Left)
            {
                return new SyncFolderDefinition(name, rootPath, SyncFolder.Left);
            }
            else if(xmlNode.Name == XmlNames.Right)
            {
                return new SyncFolderDefinition(name, rootPath, SyncFolder.Right);
            }
            else
            {
                throw new ConfigurationException(String.Format("Cannot parse xml element '{0} as SyncFolderDefinition'",
                                                               xmlNode.Name.LocalName));
            }            
        }

        #endregion SyncFolderDefinition

        #region Global Properties

        long ReadTimeStampMargin(XElement element)
        {
            return long.Parse(element.Attribute("ms").Value);
        }

        #endregion Global Properties

        #region Include

        void ReadInclude(XElement includeElement, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            var includePath = pathResolver.GetAbsolutePath(includeElement.RequireAttributeValue(XmlAttributeNames.Path)); 

            if(!File.Exists(includePath))
            {
                throw new ConfigurationException(String.Format("Included configuration file '{0}' not found", includePath));
            }

            var configurationDocument = XDocument.Load(includePath);

            var includePathResolver = new PathResolver(Path.GetDirectoryName(includePath));
            ReadSyncConfiguration(configurationDocument, configuration, includePathResolver);

        }

        #endregion

        #region Filter     

        IFilter ReadFilter(XElement filterNode)
        {           
            var name = filterNode.RequireAttributeValue(XmlAttributeNames.Name);
            if(filterNode.Elements().Count() != 1)
            {
                throw new ConfigurationException(String.Format("Invalid configuration. Expected a single root expression in filter '{0}'", name));
            }

            var rootExpression = ReadFilterExpression(filterNode.Elements().First());

            return new Filter(name, rootExpression);         
        }


        IFilterExpression ReadFilterExpression(XElement filterExpressionNode)
        {
            if(filterExpressionNode.Name == XmlNames.Regex)
            {
                var pattern = filterExpressionNode.RequireAttributeValue(XmlAttributeNames.Pattern);
                return new RegexFilterExpression(pattern);
            }
            else if(filterExpressionNode.Name == XmlNames.CompareState)
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
                if(filterExpressionNode.Elements().Count() != 1)
                {
                    throw new ConfigurationException("Invalid configuration, expected a single expression in 'not' expression");
                }

                var negatedExpression = ReadFilterExpression(filterExpressionNode.Elements().First());
                return new NotFilterExpression(negatedExpression);
            }
            else
            {
                throw new ConfigurationException(String.Format("Unknown filter-expression '{0}'", filterExpressionNode.Name.LocalName));
            }
        }
   
        #endregion Filter

        #region Actions

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
                configuration.AddAction(ReadExportAction(element, configuration,pathResolver));
            }
            else if (element.Name == XmlNames.Import)
            {
                configuration.AddAction(ReadImportAction(element, configuration, pathResolver));
            }
            else if (element.Name == XmlNames.ReadSyncState)
            {
                configuration.AddAction(ReadReadSyncStateAction(element, configuration,pathResolver));
            }
            else if (element.Name == XmlNames.WriteSyncState)
            {
                configuration.AddAction(ReadWriteSyncStateAction(element, configuration, pathResolver));
            }
            else if (element.Name == XmlNames.ApplyFilter)
            {
                configuration.AddAction(ReadApplyFilerAction(element,configuration));
            }
            else if (element.Name == XmlNames.Copy)
            {
                configuration.AddAction(ReadCopyAction(element, configuration));
            }
            else if(element.Name == XmlNames.AcquireLock)
            {
                configuration.AddAction(ReadAquireLockAction(element, configuration, pathResolver));
            }
            else if(element.Name == XmlNames.ReleaseLock)
            {
                configuration.AddAction(ReadReleaseLockAction(element, configuration, pathResolver));
            }
            else if(element.Name == XmlNames.Sleep)
            {
                configuration.AddAction(ReadSleepAction(element,configuration));
            }
            else
            {
                throw new ConfigurationException("Unknown element " + element.Name.LocalName + " in Configuration");
            }
        }

        IAction ReadCompareAction(XElement actionElement, ISyncConfiguration configuration)
        {
            var enabled = ReadActionEnabled(actionElement);            
            return new CompareAction(enabled, configuration);
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

        SyncFolder ReadActionSyncFolder(XElement actionElement)
        {
            return ParseSource(actionElement.RequireAttributeValue(XmlAttributeNames.SyncFolder));
        }

        void ApplyCommonImportExportActionProperties(XElement actionElement, ImportExportAction actionInstance, ISyncConfiguration configuration, IPathResolver pathResolver)
        {
            //TODO: checks here should already be handled by the strict schema, verify and remove checks
            if (actionElement.Element(XmlNames.MaxTransferSize) != null)
            {
                throw new ConfigurationException("You cannot specify MaxTransferSize when referencing transfer locations by name");
            }
            else if (actionElement.Element(XmlNames.MaxTransferSizeParent) != null)
            {
                throw new ConfigurationException("You cannot specify MaxTransferSizeParent when referencing transfer locations by name");
            }

            actionInstance.TransferLocationName = actionElement.RequireAttributeValue(XmlAttributeNames.TransferLocationName);
            actionInstance.TransferLocationSubPath = actionElement.RequireAttributeValue(XmlAttributeNames.TransferLocationSubPath);                       
        }

        ByteSize.ByteSize ReadByteSize(XElement byteSizeElement)
        {
            var teraByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.TeraByte);
            var gigaByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.GigaByte);
            var megaByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.MegaByte);
            var kiloByte = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.KiloByte);
            var bytes = byteSizeElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Byte);


            return ByteSize.ByteSize.FromBytes(bytes)
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
            if (value == "0")
            {
                return false;
            }
            else if(value == "1")
            {
                return true;
            }
            else
            {
                return bool.Parse(value);
            }
        }

        string ReadActionInputFilterName(XElement actionElement)
        {
            var inputFilterAttribute = actionElement.Attribute(XmlAttributeNames.InputFilter);
            if (inputFilterAttribute != null)
            {
                return inputFilterAttribute.Value;
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
            if(actionElement.Element(XmlNames.Timeout) != null)
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

        #endregion Actions

        #region TransferLocation

        TransferLocation ReadTransferLocation(XElement transferLocationElement, IPathResolver pathResolver)
        {
            ByteSize.ByteSize? maximumSize = null;
            if(transferLocationElement.Element(XmlNames.MaximumSize) != null)
            {
                maximumSize = ReadByteSize(transferLocationElement.Element(XmlNames.MaximumSize));
            }

            return new TransferLocation(transferLocationElement.RequireAttributeValue(XmlAttributeNames.Name),
                pathResolver.GetAbsolutePath(transferLocationElement.RequireAttributeValue(XmlAttributeNames.Path)),
                maximumSize);
        }
        
        #endregion

        #region Enum Parsing

        CompareState ParseCompareState(string value)
        {
            CompareState state;
            if (!Enum.TryParse<CompareState>(value, true, out state))
            {
                throw new ArgumentException("Could not parse '" + value + "' as FileState");
            }
            return state;
        }

        TransferState ParseTransferState(string value)
        {
            TransferState state;
            if (!Enum.TryParse<TransferState>(value, true, out state))
            {
                throw new ArgumentException("Could not parse '" + value + "' as TransferState");
            }
            return state;
        }

        SyncFolder ParseSource(string value)
        {
            SyncFolder source;
            if (!Enum.TryParse<SyncFolder>(value, true, out source))
            {
                throw new ArgumentException("Could not parse '" + value + "' as Source");
            }
            return source;
        }

        #endregion Enum Parsing

        #region Timespan

        TimeSpan ReadTimeSpan(XElement timespanElement)
        {
            var hours = (int) timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Hours);
            var minutes = (int)timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Minutes);
            var seconds = (int)timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.Seconds);
            var milliSeconds = (int)timespanElement.ReadLongAttributeValueOrDefault(XmlAttributeNames.MilliSeconds);

            return new TimeSpan(0, hours, minutes, seconds, milliSeconds);            
        }

        #endregion

        XmlSchemaSet GetConfigutationSchema()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(s_ConfigurationSchema))
            {
                var schemaSet = new XmlSchemaSet();

                schemaSet.Add(null, XmlReader.Create(stream));

                return schemaSet;
            }
        }

        #endregion Private Implementation

    }
}
