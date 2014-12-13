using ServerSync.Core.Compare;
using ServerSync.Core.Copy;
using ServerSync.Core.Filters;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
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

            try
            {
                ReadSyncConfiguration(configFile, configuration);
            }
            catch(ArgumentException ex)
            {
                throw new ConfigurationException("Error reading configuration", ex);
            }

            return configuration;
        }

        #endregion Public Methods


        #region Private Implementation

        void ReadSyncConfiguration(XDocument configFile, SyncConfiguration configuration)
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
                    configuration.Left = ReadSyncFolderDefinition(element);
                }
                else if (element.Name == XmlNames.Right)
                {
                    configuration.Right = ReadSyncFolderDefinition(element);
                }
                else if (element.Name == XmlNames.TimeStampMargin)
                {
                    configuration.TimeStampMargin = ReadTimeStampMargin(element);
                }
                else if (element.Name == XmlNames.Filter)
                {
                    var filter = ReadFilter(element);
                    configuration.AddFilter(filter);
                }
                else if (element.Name == XmlNames.Compare)
                {
                    configuration.AddAction(ReadCompareAction(element));
                }
                else if (element.Name == XmlNames.Export)
                {
                    configuration.AddAction(ReadExportAction(element));
                }
                else if (element.Name == XmlNames.Import)
                {
                    configuration.AddAction(ReadImportAction(element));
                }
                else if (element.Name == XmlNames.ReadSyncState)
                {
                    configuration.AddAction(ReadReadSyncStateAction(element));
                }
                else if (element.Name == XmlNames.WriteSyncState)
                {
                    configuration.AddAction(ReadWriteSyncStateAction(element));
                }
                else if (element.Name == XmlNames.ApplyFilter)
                {
                    configuration.AddAction(ReadApplyFilerAction(element));
                }
                else if (element.Name == XmlNames.Copy)
                {
                    configuration.AddAction(ReadCopyAction(element));
                }
                else
                {
                    throw new ConfigurationException("Unknown element " + element.Name.LocalName + " in Configuration");
                }
            }
        }

        #region SyncFolderDefinition

        SyncFolderDefinition ReadSyncFolderDefinition(XElement xmlNode)
        {
            var result = new SyncFolderDefinition();
            result.Name = xmlNode.Attribute(XmlAttributeNames.Name).Value;
            result.RootPath = xmlNode.Attribute(XmlAttributeNames.RootPath).Value;
            return result;
        }

        #endregion SyncFolderDefinition

        #region Global Properties

        long ReadTimeStampMargin(XElement element)
        {
            return long.Parse(element.Attribute("ms").Value);
        }

        #endregion Global Properties

        #region Filter

        IEnumerable<IFilterElement> ReadFilterElementList(XElement listNode)
        {
            List<IFilterElement> filterElements = new List<IFilterElement>();

            foreach (var elementNode in listNode.Elements())
            {
                if(elementNode.Name == XmlNames.Regex)
                {
                    var pattern = elementNode.Attribute(XmlAttributeNames.Pattern).Value;
                    filterElements.Add(new RegexFilterElement(pattern));
                }
                else if(elementNode.Name == XmlNames.CompareState)
                {
                    var compareState = ParseCompareState(elementNode.RequireAttributeValue(XmlAttributeNames.Value));
                    filterElements.Add(new CompareStateFilterElement(compareState));
                }
                else if(elementNode.Name == XmlNames.TransferState)
                {
                    var transferState = ParseTransferState(elementNode.RequireAttributeValue(XmlAttributeNames.Value));
                    filterElements.Add(new TransferStateFilterElement(transferState));
                }
                else
                {
                    throw new ConfigurationException("Unimplemented filter element: " + elementNode.Name.LocalName);
                }
            }

            return filterElements;
        }

        Filter ReadFilter(XElement filterNode)
        {
            var newFilter = new Filter();

            if (filterNode.Element(XmlNames.Include) != null)
            {
                newFilter.IncludeRules = ReadFilterElementList(filterNode.Element(XmlNames.Include));
            }
            if (filterNode.Element(XmlNames.Exclude) != null)
            {
                newFilter.ExcludeRules = ReadFilterElementList(filterNode.Element(XmlNames.Exclude));
            }
            newFilter.Name = filterNode.Attribute(XmlAttributeNames.Name).Value;

            return newFilter;
        }

        #endregion Filter

        #region Actions

        IAction ReadCompareAction(XElement actionElement)
        {
            var action = new CompareAction();
            ApplyCommonActionProperties(actionElement, action);
            return action;
        }

        IAction ReadExportAction(XElement actionElement)
        {
            var action = new ExportAction();
            ApplyCommonImportExportActionProperties(actionElement, action);
            return action;
        }

        IAction ReadImportAction(XElement actionElement)
        {
            var action = new ImportAction();
            ApplyCommonImportExportActionProperties(actionElement, action);
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

        void ApplyCommonImportExportActionProperties(XElement actionElement, ImportExportAction actionInstance)
        {
            ApplyCommonIOActionProperties(actionElement, actionInstance);

            actionInstance.TransferLocation = actionElement.RequireAttributeValue(XmlAttributeNames.TransferLocation);

            if (actionElement.Element(XmlNames.MaxTransferSize) != null)
            {
                actionInstance.MaxTransferSize = ReadByteSize(actionElement.Element(XmlNames.MaxTransferSize));
            }
            else if (actionElement.Element(XmlNames.MaxTransferSizeParent) != null)
            {
                actionInstance.MaxTransferSizeParent = ReadByteSize(actionElement.Element(XmlNames.MaxTransferSizeParent));
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

        IAction ReadReadSyncStateAction(XElement actionElement)
        {
            var action = new ReadSyncStateAction();
            ApplyCommonActionProperties(actionElement, action);
            action.FileName = actionElement.RequireAttributeValue(XmlAttributeNames.FileName);
            return action;
        }

        IAction ReadWriteSyncStateAction(XElement actionElement)
        {
            var action = new WriteSyncStateAction();
            ApplyCommonActionProperties(actionElement, action);
            action.FileName = actionElement.RequireAttributeValue(XmlAttributeNames.FileName);
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

        #endregion Actions

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
