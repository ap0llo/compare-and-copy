using ServerSync.Core.Compare;
using ServerSync.Core.Copy;
using ServerSync.Core.Filters;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.Configuration
{
    public class ConfigurationReader
    {
        #region Public  Methods

        public SyncConfiguration ReadConfiguration(string fileName)
        {
            XDocument configFile = XDocument.Load(fileName);

            SyncConfiguration configuration = new SyncConfiguration();

            ReadSyncConfiguration(configFile, configuration);

            return configuration;            
        }

        #endregion Public Methods


        #region Private Implementation

        private void ReadSyncConfiguration(XDocument configFile, SyncConfiguration configuration)
        {
            foreach (var element in configFile.Root.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case XmlConstants.Left:
                        configuration.Left = ReadSyncFolderDefinition(element);
                        break;

                    case XmlConstants.Right:
                        configuration.Right = ReadSyncFolderDefinition(element);
                        break;

                    case XmlConstants.TimeStampMargin:
                        configuration.TimeStampMargin = ReadTimeStampMargin(element);
                        break;

                    case XmlConstants.Filter:
                        var filter = ReadFilter(element);
                        configuration.AddFilter(filter);
                        break;

                    case XmlConstants.Compare:
                        configuration.AddAction(ReadCompareAction(element));
                        break;

                    case XmlConstants.Export:
                        configuration.AddAction(ReadExportAction(element));
                        break;
                    case XmlConstants.Import:
                        configuration.AddAction(ReadImportAction(element));
                        break;

                    case XmlConstants.ReadSyncState:
                        configuration.AddAction(ReadReadSyncStateAction(element));
                        break;

                    case XmlConstants.WriteSyncState:
                        configuration.AddAction(ReadWriteSyncStateAction(element));
                        break;     
            
                    case XmlConstants.ApplyFilter:
                        configuration.AddAction(ReadApplyFilerAction(element));
                        break;

                    case XmlConstants.Copy:
                        configuration.AddAction(ReadCopyAction(element));
                        break;

                    default:
                        throw new NotImplementedException("Unknwon element " + element.Name.LocalName + " in Configuration");                        
                }
            }
            
        }

        #region SyncFolderDefinition

        private SyncFolderDefinition ReadSyncFolderDefinition(XElement xmlNode)
        {
            var result = new SyncFolderDefinition();
            result.Name = xmlNode.Attribute(XmlConstants.Name).Value;
            result.RootPath = xmlNode.Attribute(XmlConstants.RootPath).Value;
            return result;
        }

        #endregion SyncFolderDefinition

        #region Global Properties

        private long ReadTimeStampMargin(XElement element)
        {
            return long.Parse(element.Attribute("ms").Value);
            
        }

        #endregion Global Properties

        #region Filter

        private IEnumerable<IFilterElement> ReadFilterElementList(XElement listNode)
        {
            List<IFilterElement> filterElements = new List<IFilterElement>();

            foreach (var elementNode in listNode.Elements())
            {
                switch (elementNode.Name.LocalName)
                {
                    case XmlConstants.Regex:
                        var pattern = elementNode.Attribute(XmlConstants.Pattern).Value;
                        filterElements.Add(new RegexFilterElement(pattern));
                        break;

                    case XmlConstants.CompareState:
                        var compareState = ParseCompareState(elementNode.RequireAttributeValue(XmlConstants.Value));
                        filterElements.Add(new CompareStateFilterElement(compareState));
                        break;

                    case XmlConstants.TransferState :
                        var transferState = ParseTransferState(elementNode.RequireAttributeValue(XmlConstants.Value));
                        filterElements.Add(new TransferStateFilterElement(transferState));
                        break;

                    default:
                        throw new NotImplementedException("Unimplemented filter element: " + elementNode.Name.LocalName);

                }
            }

            return filterElements;
        }

        private Filter ReadFilter(XElement filterNode)
        {
            var newFilter = new Filter();

            newFilter.IncludeRules = ReadFilterElementList(filterNode.Element(XmlConstants.Include));
            newFilter.ExcludeRules = ReadFilterElementList(filterNode.Element(XmlConstants.Exclude));
            newFilter.Name = filterNode.Attribute(XmlConstants.Name).Value;

            return newFilter;
        }

        #endregion Filter

        #region Actions

        private IAction ReadCompareAction(XElement actionElement)
        {            
            var action = new CompareAction();
            ApplyCommonActionProperties(actionElement, action);
            return action;
        }

        private IAction ReadExportAction(XElement actionElement)
        {
            var action = new ExportAction();
            ApplyCommonImportExportActionProperties(actionElement, action);
            return action;

        }

        private IAction ReadImportAction(XElement actionElement)
        {
            var action = new ImportAction();
            ApplyCommonImportExportActionProperties(actionElement, action);
            return action;
        }

        private IAction ReadCopyAction(XElement actionElement)
        {
            var actionInstance = new CopyAction();
            ApplyCommonIOActionProperties(actionElement, actionInstance);
            return actionInstance;
        }

        private void ApplyCommonIOActionProperties(XElement actionElement, IOAction actionInstance)
        {
            ApplyCommonActionProperties(actionElement, actionInstance);
            actionInstance.SyncFolder = ParseSource(actionElement.RequireAttributeValue(XmlConstants.SyncFolder));
        }

        private void ApplyCommonImportExportActionProperties(XElement actionElement, ImportExportAction actionInstance)
        {

            ApplyCommonIOActionProperties(actionElement, actionInstance);
            
            actionInstance.TransferLocation = actionElement.RequireAttributeValue(XmlConstants.TransferLocation);
        }     



        private IAction ReadReadSyncStateAction(XElement actionElement)
        {
            var action = new ReadSyncStateAction();
            ApplyCommonActionProperties(actionElement, action);
            action.FileName = actionElement.RequireAttributeValue(XmlConstants.FileName);
            return action;
        }

        private IAction ReadWriteSyncStateAction(XElement actionElement)
        {
            var action = new WriteSyncStateAction();
            ApplyCommonActionProperties(actionElement, action);
            action.FileName = actionElement.RequireAttributeValue(XmlConstants.FileName);
            return action;       
        }       

        private IAction ReadApplyFilerAction(XElement actionElement)
        {
            var action = new ApplyFilterAction();
            ApplyCommonActionProperties(actionElement, action);
            return action;
        }

        private void ApplyCommonActionProperties(XElement actionElement, IAction actionInstance)
        {
            bool enable = bool.Parse(actionElement.RequireAttributeValue(XmlConstants.Enable));
            actionInstance.IsEnabled = enable;

            var inputFilterAttribute = actionElement.Attribute(XmlConstants.InputFilter);
            if (inputFilterAttribute != null)
            {
                actionInstance.InputFilterName = inputFilterAttribute.Value;
            }
        }


        #endregion Actions

        #region Enum Parsing

        private CompareState ParseCompareState(string value)
        {
            CompareState state;
            if (!Enum.TryParse<CompareState>(value, true, out state))
            {
                throw new ArgumentException("Could not parse '" + value + "' as FileState");
            }
            return state;
        }

        private TransferState ParseTransferState(string value)
        {
            TransferState state;
            if (!Enum.TryParse<TransferState>(value, true, out state))
            {
                throw new ArgumentException("Could not parse '" + value + "' as TransferState");
            }
            return state;
        }

        private SyncFolder ParseSource(string value)
        {
            SyncFolder source;
            if (!Enum.TryParse<SyncFolder>(value, true, out source))
            {
                throw new ArgumentException("Could not parse '" + value + "' as Source");
            }
            return source;
        }

        #endregion Enum Parsing

        #region Xml Constants

        private class XmlConstants
        {
            public const string Left = "left";
            
            public const string Right = "right";
            
            public const string Name = "name";
          
            public const string RootPath = "rootPath";

            public const string TimeStampMargin = "timeStampMargin";

            public const string Regex = "regex";           
            
            public const string Filter = "filter";
            
            public const string Include = "include";

            public const string Exclude = "exclude";
            
            public const string Enable = "enable";
           
            public const string CompareState = "compareState";
            
            public const string TransferState = "transferState";
            
            public const string Pattern = "pattern";

            public const string TransferLocation = "transferLocation";
            
            public const string SyncFolder = "syncFolder";

            public const string FileName = "fileName";            

            public const string InputFilter = "inputFilter";

            public const string Value = "value";

            //Action Names

            public const string Compare = "compare";
            
            public const string Export = "export";
            
            public const string Import = "import";
            
            public const string ReadSyncState = "readSyncState";
            
            public const string WriteSyncState = "writeSyncState";
            
            public const string ApplyFilter = "applyFilter";

            public const string Copy = "copy";

        }

        #endregion Xml Constants

        #endregion Private Implementation
    }
}
