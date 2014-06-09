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
        }

        #endregion Private Implementation
    }
}
