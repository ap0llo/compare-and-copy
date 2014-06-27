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
        
        public SyncConfiguration ReadConfiguration(string fileName)
        {
            XDocument configFile = XDocument.Load(fileName);

            SyncConfiguration configuration = new SyncConfiguration();

            ReadSyncConfiguration(configFile, configuration);

            return configuration;            
        }


        #region Private Implementation

        private void ReadSyncConfiguration(XDocument configFile, SyncConfiguration configuration)
        {
            foreach (var element in configFile.Root.Elements())
            {
                switch (element.Name.LocalName)
                {
                    case XmlConstants.Left:
                        configuration.Left = ReadSyncFolder(element);
                        break;

                    case XmlConstants.Right:
                        configuration.Right = ReadSyncFolder(element);
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

                    case XmlConstants.ReadSyncState:
                        configuration.AddAction(ReadReadSyncStateAction(element));
                        break;

                    case XmlConstants.WriteSyncState:
                        configuration.AddAction(ReadWriteSyncStateAction(element));
                        break;

                    case XmlConstants.ApplyFilter:
                        configuration.AddAction(ReadApplyFilterAction(element));
                        break;

                    default:
                        throw new NotImplementedException("Unknwon element " + element.Name.LocalName + " in Configuration");                        
                }
            }
            
        }

        private SyncFolder ReadSyncFolder(XElement xmlNode)
        {
            var result = new SyncFolder();
            result.Name = xmlNode.Attribute(XmlConstants.Name).Value;
            result.RootPath = xmlNode.Attribute(XmlConstants.RootPath).Value;
            return result;
        }

        private long ReadTimeStampMargin(XElement element)
        {
            return long.Parse(element.Attribute("ms").Value);
            
        }
         
        private Filter ReadFilter(XElement filterNode)
        {
            var newFilter = new Filter();

            newFilter.IncludeRules = ReadFilterElementList(filterNode.Element(XmlConstants.Include));
            newFilter.ExcludeRules = ReadFilterElementList(filterNode.Element(XmlConstants.Exclude));
            newFilter.Name = filterNode.Attribute(XmlConstants.Name).Value;

            return newFilter;
        }

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

                    case XmlConstants.FileState:
                        var stateString = elementNode.Attribute(XmlConstants.State).Value;
                        var state = ParseFileState(stateString);
                        filterElements.Add(new FileStateFilterElement(state));
                        break;

                    default:
                        throw new NotImplementedException("Unimplemented filter element: " + elementNode.Name.LocalName);

                }
            }

            return filterElements;
        }

        private IAction ReadCompareAction(XElement actionElement)
        {
            bool enable = bool.Parse(actionElement.Attribute(XmlConstants.Enable).Value);
            var action = new CompareAction();
            action.IsEnabled = enable;
            return action;
        }

        private IAction ReadExportAction(XElement actionElement)
        {
            bool enable = bool.Parse(actionElement.Attribute(XmlConstants.Enable).Value);
            var fileStateStr = actionElement.Attribute(XmlConstants.FileState).Value;           
            var dir = actionElement.Attribute(XmlConstants.TargetDirectory).Value;
            var sourceStr = actionElement.Attribute(XmlConstants.Source).Value;

            var action = new ExportAction();
            action.IsEnabled = enable;
            action.FileState = ParseFileState(fileStateStr);
            action.TargetDirectory = dir;            
            action.Source = ParseSource(sourceStr);
            return action;

        }

        private IAction ReadReadSyncStateAction(XElement actionElement)
        {
            bool enable = bool.Parse(actionElement.Attribute(XmlConstants.Enable).Value);
            var fileName = actionElement.Attribute(XmlConstants.FileName).Value;

            return new ReadSyncStateAction() { IsEnabled = enable, FileName = fileName};            
        }

        private IAction ReadWriteSyncStateAction(XElement actionElement)
        {
            bool enable = bool.Parse(actionElement.Attribute(XmlConstants.Enable).Value);
            var fileName = actionElement.Attribute(XmlConstants.FileName).Value;

            return new WriteSyncStateAction() { IsEnabled = enable, FileName = fileName };          
        }

        private IAction ReadApplyFilterAction(XElement actionElement)
        {
            bool enable = bool.Parse(actionElement.Attribute(XmlConstants.Enable).Value);
            var filterName = actionElement.Attribute(XmlConstants.FilterName).Value;

            return new ApplyFilterAction() { IsEnabled = enable, FilterName = filterName };     
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


            public const string TimeStampMargin = "timeStampMargin";

            public const string Regex = "regex";
            public const string FilterList = "filterList";
            public const string Filter = "filter";
            public const string Include = "include";
            public const string Exclude = "exclude";
            
            public const string Enable = "enable";

            public const string FileState = "fileState";
            public const string TargetDirectory = "targetDirectory";
            public const string SetState = "setState";
            public const string Source = "source";

            public const string FileName = "fileName";

            public const string FilterName = "filterName";

            //Action Names

            public const string Compare = "compare";
            public const string Export = "export";
            public const string ReadSyncState = "readSyncState";
            public const string WriteSyncState = "writeSyncState";
            public const string ApplyFilter = "applyFilter";
            public const string State ="state";


            
        }

        #endregion Private Implementation
    }
}
