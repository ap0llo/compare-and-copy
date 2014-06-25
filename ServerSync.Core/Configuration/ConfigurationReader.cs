using ServerSync.Core.Compare;
using ServerSync.Core.Copy;
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
        private const long TIMESTAMPMARGIN_DEFAULT = 0;
        private const string FILTER_DEFAULT = "^[.]*$";
        private const string LOGDIRECTORY_DEFAULT = ".";


        public SyncConfiguration ReadConfiguration(string fileName)
        {
            XDocument configFile = XDocument.Load(fileName);

            SyncConfiguration configuration = new SyncConfiguration();
            configuration = ReadSyncFolders(configFile, configuration);
            configuration = ReadLogDirectory(configFile, configuration);            
            configuration = ReadTimeStampMargin(configFile, configuration);
            configuration = ReadFilterList(configFile, configuration);
            configuration = ReadActionList(configFile, configuration);

            return configuration;            
        }


        #region Private Implementation

        private SyncConfiguration ReadSyncFolders(XDocument document, SyncConfiguration currentConfiguration)
        {
            currentConfiguration.Left = ReadSyncFolder(document.Root.Element(XmlConstants.Left));
            currentConfiguration.Right = ReadSyncFolder(document.Root.Element(XmlConstants.Right));

            return currentConfiguration;
        }

        private SyncFolder ReadSyncFolder(XElement xmlNode)
        {
            var result = new SyncFolder();
            result.Name = xmlNode.Attribute(XmlConstants.Name).Value;
            result.RootPath = xmlNode.Attribute(XmlConstants.RootPath).Value;
            return result;
        }

        private SyncConfiguration ReadTimeStampMargin(XDocument configDocument, SyncConfiguration currentConfiguration)
        {
            var node = configDocument.Root.Element(XmlConstants.TimeStampMargin);

            if(node == null)
            {
                currentConfiguration.TimeStampMargin = TIMESTAMPMARGIN_DEFAULT;
            }
            else
            {
                currentConfiguration.TimeStampMargin = long.Parse(node.Attribute("ms").Value);
            }

            return currentConfiguration;
        }

        private SyncConfiguration ReadLogDirectory(XDocument document, SyncConfiguration currentConfiguration)
        {
            var node = document.Root.Element(XmlConstants.LogDirectory);
            if(node == null)
            {
                currentConfiguration.LogDirectory = LOGDIRECTORY_DEFAULT;
            }
            else
            {
                currentConfiguration.LogDirectory = node.Value;
            }
            
            return currentConfiguration;
        }

        private SyncConfiguration ReadFilterList(XDocument document, SyncConfiguration currentConfiguration)
        {
            var filterListNode = document.Root.Element(XmlConstants.FilterList);

            //set default filter
            if(filterListNode == null || !filterListNode.Elements(XmlConstants.Filter).Any())
            {
                Filter defaultFilter = GetDefaultFilter();
                currentConfiguration.Filters = new List<Filter>(){ defaultFilter}; 
            }
            else
            {
                var filterNodes = filterListNode.Elements(XmlConstants.Filter);
                currentConfiguration.Filters = filterNodes.Select(filterNode => ReadFilter(filterNode)).ToList();
            }

            

            return currentConfiguration;
        }

        private Filter ReadFilter(XElement filterNode)
        {
            var newFilter = new Filter();

            newFilter.IncludeRules = ReadRegexRuleList(filterNode.Element(XmlConstants.Include));
            newFilter.ExcludeRules = ReadRegexRuleList(filterNode.Element(XmlConstants.Exclude));

            return newFilter;
        }

        private IEnumerable<Regex> ReadRegexRuleList(XElement listNode)
        {
            return listNode.Elements(XmlConstants.Regex).Select(node => node.Attribute(XmlConstants.Pattern).Value)
                           .Select(pattern => new Regex(pattern));
        }

        private Filter GetDefaultFilter()
        {
            Filter defaultFilter = new Filter();
            defaultFilter.IncludeRules = new List<Regex>() {
                new Regex(FILTER_DEFAULT)
            };
            return defaultFilter;
        }

        private SyncConfiguration ReadActionList(XDocument document, SyncConfiguration currentConfiguration)
        {
            var actionList = document.Root.Element(XmlConstants.ActionList);

            List<IAction> actions = new List<IAction>();

            foreach(var actionNode in actionList.Elements())
            {
                IAction action;
                switch (actionNode.Name.LocalName)
                {
                    case XmlConstants.Compare:
                        action = ReadCompareAction(actionNode);
                        break;
                    case XmlConstants.Copy:
                        action = ReadCopyAction(actionNode);
                        break;
                    default:
                        throw new NotSupportedException("Unknown action name: " + actionNode.Name.LocalName);
                }                
                actions.Add(action);
            }

            currentConfiguration.Actions = actions;
            return currentConfiguration;
        }

        private IAction ReadCompareAction(XElement actionElement)
        {
            bool enable = bool.Parse(actionElement.Attribute(XmlConstants.Enable).Value);
            var action = new CompareAction();
            action.IsEnabled = enable;
            return action;
        }


        private IAction ReadCopyAction(XElement actionElement)
        {
            bool enable = bool.Parse(actionElement.Attribute(XmlConstants.Enable).Value);
            var itemStateStr = actionElement.Attribute(XmlConstants.ItemType).Value;
            var setStateStr = actionElement.Attribute(XmlConstants.SetState).Value;
            var dir = actionElement.Attribute(XmlConstants.TargetDirectory).Value;
            var sourceStr = actionElement.Attribute(XmlConstants.Source).Value;

            var action = new CopyAction();
            action.IsEnabled = enable;
            action.ItemType = ParseFileState(itemStateStr);
            action.TargetDirectory = dir;
            action.SetStateTo = ParseFileState(setStateStr);
            action.Source = ParseSource(sourceStr);
            return action;

        }

        private FileState ParseFileState(string value)
        {
            FileState fileState;
            if (!Enum.TryParse<FileState>(value, true, out fileState))
            {
                throw new ArgumentException("Could not parse '" + value + "' as FileState");
            }
            return fileState;
        }

        private Source ParseSource(string value)
        {
            Source source;
            if (!Enum.TryParse<Source>(value, true, out source))
            {
                throw new ArgumentException("Could not parse '" + value + "' as Source");
            }
            return source;
        }

        private class XmlConstants
        {
            public const string Left = "left";
            public const string Right = "right";
            
            public const string Name = "name";
            public const string RootPath = "rootPath";

            public const string Pattern = "pattern";

            public const string LogDirectory = "logDirectory";

            public const string TimeStampMargin = "timeStampMargin";

            public const string Regex = "regex";
            public const string FilterList = "filterList";
            public const string Filter = "filter";
            public const string Include = "include";
            public const string Exclude = "exclude";
            public const string ActionList = "actionList";
            public const string Compare = "compare";
            public const string Copy = "copy";

            public const string Enable = "enable";

            public const string ItemType = "itemType";
            public const string TargetDirectory = "targetDirectory";
            public const string SetState = "setState";
            public const string Source = "source";
        }

        #endregion Private Implementation
    }
}
