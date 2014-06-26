using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.State
{
    public class SyncStateReader
    {

        public SyncState ReadSyncState(string fileName)
        {
            if(!File.Exists(fileName))
            {
                return new SyncState();
            }

            var document = XDocument.Load(fileName);
            var files = document.Descendants(XmlConstants.File).Select(ReadFileItem);

            return new SyncState() { Files = files.ToList() };
        }


        private FileItem ReadFileItem( XElement item)
        {
            string path = item.Attribute(XmlConstants.Path).Value;
            string stateStr = item.Attribute(XmlConstants.Type).Value;

            if(String.IsNullOrEmpty(path))
            {
                throw new SyncStateException("Empty path found in item list");
            }

            FileState fileState;
            if(!Enum.TryParse<FileState>(stateStr, true, out fileState))
            {
                throw new SyncStateException("Unknwon type: " + stateStr);
            }

            return new FileItem() { RelativePath = path, State = fileState };      
        }

    }
}
