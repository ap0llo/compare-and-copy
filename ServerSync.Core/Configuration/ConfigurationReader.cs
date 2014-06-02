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
            configuration.Left = ReadSyncFolder(configFile.Root.Element(XmlConstants.Left));
            configuration.Right = ReadSyncFolder(configFile.Root.Element(XmlConstants.Right));
            configuration.ExcludedFiles = ReadFileFilter(configFile.Root.Element(XmlConstants.FileFilter)).ToList();
            configuration.IncludeFolders = ReadDirectoryFilter(configFile.Root.Element(XmlConstants.DirectoryFilter));
            configuration.LogDirectory = configFile.Root.Element(XmlConstants.LogDirectory).Value;
            
            configuration.TimeStampMargin = ReadTimeStampMargin(configFile);

            return configuration;            
        }

        private SyncFolder ReadSyncFolder(XElement xmlNode)
        {
            var result = new SyncFolder();
            result.Name = xmlNode.Attribute(XmlConstants.Name).Value;
            result.RootPath = xmlNode.Attribute(XmlConstants.RootPath).Value;
            return result;
        }

        private IEnumerable<Regex> ReadFileFilter(XElement xmlNode)
        {
            return xmlNode.Elements(XmlConstants.ExcludeFile)
                           .Select(node => node.Attribute(XmlConstants.Pattern).Value)
                           .Select(pattern => new Regex(pattern));
        }

        private IEnumerable<Regex> ReadDirectoryFilter(XElement xmlNode)
        {
            return xmlNode.Elements(XmlConstants.IncludeDirectory)
                          .Select(node => node.Attribute(XmlConstants.Pattern).Value)
                          .Select(pattern => new Regex(pattern));
        }

        private long ReadTimeStampMargin(XDocument configDocument)
        {
            var node = configDocument.Root.Element(XmlConstants.TimeStampMargin);
            return long.Parse(node.Attribute("ms").Value);
        }



        private class XmlConstants
        {
            public const string Left = "left";
            public const string Right = "right";
            public const string FileFilter = "fileFilter";

            public const string Name = "name";
            public const string RootPath = "rootPath";

            public const string ExcludeFile = "excludeFile";
            public const string Pattern = "pattern";

            public const string DirectoryFilter = "directoryFilter";
            public const string IncludeDirectory = "includeDirectory";

            public const string LogDirectory = "logDirectory";

            public const string TimeStampMargin = "timeStampMargin";
        }

    }
}
