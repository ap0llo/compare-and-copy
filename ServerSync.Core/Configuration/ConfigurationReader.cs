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

namespace ServerSync.Core.Configuration
{
    public class ConfigurationReader
    {

        #region Constants

        const string s_ConfigurationSchema = "ServerSync.Core.Configuration.ConfigurationSchema.xsd";

        #endregion


        #region Public  Methods

        /// <summary>
        /// Reads sync configuration from the specified file
        /// </summary>
        public SyncConfiguration ReadConfiguration(string fileName)
        {
            XDocument configFile = XDocument.Load(fileName);

            SyncConfiguration configuration = new SyncConfiguration();

            var pathResolver = new PathResolver(Path.GetDirectoryName(fileName));
            try
            {
                ReadSyncConfiguration(configFile, configuration, pathResolver);
            }
            catch(ArgumentException ex)
            {
                throw new ConfigurationException("Error reading configuration", ex);
            }

            return configuration;
        }

        #endregion Public Methods


        #region Private Implementation

        void ReadSyncConfiguration(XDocument configFile, SyncConfiguration configuration, IPathResolver pathResolver)
        {

            //support for legacy configuration files which did not include any xml namespace
            if(String.IsNullOrEmpty(configFile.Root.Name.NamespaceName))
            {
                configFile.Root.ReplaceNamespace("", XmlNames.GetNamespace());
            }

            configFile.Validate(GetConfigutationSchema(), (o, e) => { throw new ConfigurationException(e.Message); });

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
                else if (element.Name == XmlNames.Compare)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.Export)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.Import)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if(element.Name == XmlNames.TransferLocation)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.ReadSyncState)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.WriteSyncState)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.ApplyFilter)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.Copy)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if(element.Name == XmlNames.Actions)
                {
                    ReadActionList(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.AcquireLock)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.ReleaseLock)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else if (element.Name == XmlNames.Sleep)
                {
                    ReadAction(element, configuration, pathResolver);
                }
                else
                {
                    throw new ConfigurationException("Unknown element " + element.Name.LocalName + " in Configuration");
                }
            }
        }

        #region SyncFolderDefinition

        SyncFolderDefinition ReadSyncFolderDefinition(XElement xmlNode, IPathResolver pathResolver)
        {
            var result = new SyncFolderDefinition();
            result.Name = xmlNode.Attribute(XmlAttributeNames.Name).Value;
            result.RootPath = pathResolver.GetAbsolutePath(xmlNode.Attribute(XmlAttributeNames.RootPath).Value);
            return result;
        }

        #endregion SyncFolderDefinition

        #region Global Properties

        long ReadTimeStampMargin(XElement element)
        {
            return long.Parse(element.Attribute("ms").Value);
        }

        #endregion Global Properties

        #region Include

        void ReadInclude(XElement includeElement, SyncConfiguration configuration, IPathResolver pathResolver)
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

        IEnumerable<IFilterExpression> ReadLegacyFilterExpressionList(XElement listNode)
        {
            List<IFilterExpression> filterElements = new List<IFilterExpression>();

            foreach (var elementNode in listNode.Elements())
            {
                if(elementNode.Name == XmlNames.Regex)
                {
                    var pattern = elementNode.Attribute(XmlAttributeNames.Pattern).Value;
                    filterElements.Add(new RegexFilterExpression(pattern));
                }
                else if(elementNode.Name == XmlNames.CompareState)
                {
                    var compareState = ParseCompareState(elementNode.RequireAttributeValue(XmlAttributeNames.Value));
                    filterElements.Add(new CompareStateFilterExpression(compareState));
                }
                else if(elementNode.Name == XmlNames.TransferState)
                {
                    var transferState = ParseTransferState(elementNode.RequireAttributeValue(XmlAttributeNames.Value));
                    filterElements.Add(new TransferStateFilterExpression(transferState));
                }
                else if (elementNode.Name == XmlNames.MicroscopeQuery)
                {
                    var query = elementNode.RequireAttributeValue(XmlAttributeNames.Query);
                    filterElements.Add(new MicroscopeFilterExpression(query));
                }
                else
                {
                    throw new ConfigurationException("Unimplemented filter element: " + elementNode.Name.LocalName);
                }
            }

            return filterElements;
        }





        IFilter ReadFilter(XElement filterNode)
        {
            if(IsLegacyFilter(filterNode))
            {
                return ReadLegacyFilter(filterNode);
            }

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

        bool IsLegacyFilter(XElement filterNode)
        {
            return filterNode.Elements().Count() <= 2 &&
                (filterNode.Elements().Count(element => element.Name ==  XmlNames.Include) == 1 ||
                filterNode.Elements().Count(element => element.Name ==  XmlNames.Include) == 1);
        }

        IFilter ReadLegacyFilter(XElement filterNode)
        {
#pragma warning disable 612,618 //we need to support legacy filter for backwards compatibility

            var newFilter = new LegacyFilter();

#pragma warning restore 612, 618

            if (filterNode.Element(XmlNames.Include) != null)
            {
                newFilter.IncludeRules = ReadLegacyFilterExpressionList(filterNode.Element(XmlNames.Include));
            }
            if (filterNode.Element(XmlNames.Exclude) != null)
            {
                newFilter.ExcludeRules = ReadLegacyFilterExpressionList(filterNode.Element(XmlNames.Exclude));
            }
            newFilter.Name = filterNode.Attribute(XmlAttributeNames.Name).Value;

            return newFilter;
        }

        #endregion Filter

        #region Actions

        void ReadActionList(XElement actionListElement, SyncConfiguration configuration, IPathResolver pathResolver)
        {
            foreach (var actionElement in actionListElement.Elements())
            {
                ReadAction(actionElement, configuration, pathResolver);
            }
        }

        void ReadAction(XElement element, SyncConfiguration configuration, IPathResolver pathResolver)
        {
            if (element.Name == XmlNames.Compare)
            {
               configuration.AddAction(ReadCompareAction(element));
            }
            else if (element.Name == XmlNames.Export)
            {
                configuration.AddAction(ReadExportAction(element, configuration,pathResolver));
            }
            else if (element.Name == XmlNames.Import)
            {
                configuration.AddAction(ReadImportAction(element, configuration, pathResolver));
            }
            else if (element.Name == XmlNames.TransferLocation)
            {
                configuration.AddTransferLocation(ReadTransferLocation(element, pathResolver));
            }
            else if (element.Name == XmlNames.ReadSyncState)
            {
                configuration.AddAction(ReadReadSyncStateAction(element, pathResolver));
            }
            else if (element.Name == XmlNames.WriteSyncState)
            {
                configuration.AddAction(ReadWriteSyncStateAction(element,pathResolver));
            }
            else if (element.Name == XmlNames.ApplyFilter)
            {
                configuration.AddAction(ReadApplyFilerAction(element));
            }
            else if (element.Name == XmlNames.Copy)
            {
                configuration.AddAction(ReadCopyAction(element));
            }
            else if(element.Name == XmlNames.AcquireLock)
            {
                configuration.AddAction(ReadAquireLockAction(element, pathResolver));
            }
            else if(element.Name == XmlNames.ReleaseLock)
            {
                configuration.AddAction(ReadReleaseLockAction(element, pathResolver));
            }
            else if(element.Name == XmlNames.Sleep)
            {
                configuration.AddAction(ReadSleepAction(element));
            }
            else
            {
                throw new ConfigurationException("Unknown element " + element.Name.LocalName + " in Configuration");
            }
        }

        IAction ReadCompareAction(XElement actionElement)
        {
            var action = new CompareAction();
            ApplyCommonActionProperties(actionElement, action);
            return action;
        }

        IAction ReadExportAction(XElement actionElement, SyncConfiguration configuration, IPathResolver pathResolver)
        {
            var action = new ExportAction();
            ApplyCommonImportExportActionProperties(actionElement, action, configuration, pathResolver);
            return action;
        }

        IAction ReadImportAction(XElement actionElement, SyncConfiguration configuration, IPathResolver pathResolver)
        {
            var action = new ImportAction();
            ApplyCommonImportExportActionProperties(actionElement, action, configuration, pathResolver);
            return action;
        }

        IAction ReadCopyAction(XElement actionElement)
        {
            var actionInstance = new CopyAction();
            ApplyCommonIOActionProperties(actionElement, actionInstance);
            return actionInstance;
        }

        void ApplyCommonIOActionProperties(XElement actionElement, IOAction actionInstance)
        {
            ApplyCommonActionProperties(actionElement, actionInstance);
            actionInstance.SyncFolder = ParseSource(actionElement.RequireAttributeValue(XmlAttributeNames.SyncFolder));
        }

        void ApplyCommonImportExportActionProperties(XElement actionElement, ImportExportAction actionInstance, SyncConfiguration configuration, IPathResolver pathResolver)
        {
            ApplyCommonIOActionProperties(actionElement, actionInstance);

            if(actionElement.Attribute(XmlAttributeNames.TransferLocation) != null)
            {
                if(actionElement.Attribute(XmlAttributeNames.TransferLocationName) != null ||
                    actionElement.Attribute(XmlAttributeNames.TransferLocationSubPath) != null)
                {
                    throw new ConfigurationException("If 'transferLocation' is specified, the attributes transferLocationName and transferLocationSubPath are not allowed");
                }

                //create a new TransferLocation instance
                TransferLocation transferLocation = null;
                var transferLocationName = String.Format("TransferLocation_{0}", Guid.NewGuid());
                var transferLocationPath = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.TransferLocation));
            
                actionInstance.TransferLocationSubPath = "";
                actionInstance.TransferLocationName = transferLocationName;

                if (actionElement.Element(XmlNames.MaxTransferSize) != null)
                {
                    transferLocation = new TransferLocation(transferLocationName, transferLocationPath, ReadByteSize(actionElement.Element(XmlNames.MaxTransferSize)));                    
                }
                else if (actionElement.Element(XmlNames.MaxTransferSizeParent) != null)
                {
                    actionInstance.TransferLocationSubPath = Path.GetFileName(transferLocationPath);                                        
                    transferLocation = new TransferLocation(transferLocationName, Path.GetDirectoryName(transferLocationPath), ReadByteSize(actionElement.Element(XmlNames.MaxTransferSizeParent)));
                }
                else
                {
                    transferLocation = new TransferLocation(transferLocationName, transferLocationPath, null);
                }

                configuration.AddTransferLocation(transferLocation);
            }
            else
            {
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

        IAction ReadReadSyncStateAction(XElement actionElement, IPathResolver pathResolver)
        {
            var action = new ReadSyncStateAction();
            ApplyCommonActionProperties(actionElement, action);
            action.FileName = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.FileName));
            return action;
        }

        IAction ReadWriteSyncStateAction(XElement actionElement, IPathResolver pathResolver)
        {
            var action = new WriteSyncStateAction();
            ApplyCommonActionProperties(actionElement, action);
            action.FileName = pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.FileName));
            return action;
        }

        IAction ReadApplyFilerAction(XElement actionElement)
        {
            var action = new ApplyFilterAction();
            ApplyCommonActionProperties(actionElement, action);
            return action;
        }

        void ApplyCommonActionProperties(XElement actionElement, IAction actionInstance)
        {
            bool enable = bool.Parse(actionElement.RequireAttributeValue(XmlAttributeNames.Enable));
            actionInstance.IsEnabled = enable;

            var inputFilterAttribute = actionElement.Attribute(XmlAttributeNames.InputFilter);
            if (inputFilterAttribute != null)
            {
                actionInstance.InputFilterName = inputFilterAttribute.Value;
            }
        }

        IAction ReadAquireLockAction(XElement actionElement, IPathResolver pathResolver)
        {
            TimeSpan? timeout = null;
            if(actionElement.Element(XmlNames.Timeout) != null)
            {
                timeout = ReadTimeSpan(actionElement.Element(XmlNames.Timeout));
            }
            var action = new AcquireLockAction(pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.LockFile)), timeout);
            ApplyCommonActionProperties(actionElement, action);
            return action;
        }

        IAction ReadReleaseLockAction(XElement actionElement, IPathResolver pathResolver)
        {
            var action = new ReleaseLockAction(pathResolver.GetAbsolutePath(actionElement.RequireAttributeValue(XmlAttributeNames.LockFile)));
            ApplyCommonActionProperties(actionElement, action);
            return action;
        }

        IAction ReadSleepAction(XElement actionElement)
        {
            var action = new SleepAction(ReadTimeSpan(actionElement.Element(XmlNames.Timeout)));
            ApplyCommonActionProperties(actionElement, action);
            return action;
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
