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

            return new SyncState(files.ToList());
        }


        private FileItem ReadFileItem( XElement item)
        {
            string path = item.Attribute(XmlConstants.Path).Value;

            if(String.IsNullOrEmpty(path))
            {
                throw new SyncStateException("Empty path found in item list");
            }

            string compareStateStr = item.RequireAttributeValue(XmlConstants.CompareState);
            CompareState compareState;
            if(!Enum.TryParse<CompareState>(compareStateStr, true, out compareState))
            {
                throw new SyncStateException("Unknwon type: " + compareStateStr);
            }

            var transferStateStr = item.RequireAttributeValue(XmlConstants.TransferState);
            TransferState transferState;
            if(!Enum.TryParse<TransferState>(transferStateStr, out transferState))
            {
                throw new SyncStateException("Unknwon type: " + transferStateStr);
            }

            return new FileItem() { RelativePath = path, CompareState = compareState };      
        }

    }
}
