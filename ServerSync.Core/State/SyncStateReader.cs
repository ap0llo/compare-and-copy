using System;
using System.Collections.Generic;
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
            var document = XDocument.Load(fileName);

            var allItems = document.Descendants(XmlConstants.File).Select(ReadFileItem);

            var missingLeft = allItems.Where(tuple => tuple.Item2 == FileState.MissingLeft).Select(tuple => tuple.Item1);
            var missingRight = allItems.Where(tuple => tuple.Item2 == FileState.MissingRight).Select(tuple => tuple.Item1);
            var conflicts = allItems.Where(tuple => tuple.Item2 == FileState.Conflict).Select(tuple => tuple.Item1);
            var inTransferToLeft = allItems.Where(tuple => tuple.Item2 == FileState.InTransferToLeft).Select(tuple => tuple.Item1);
            var inTransferToRight = allItems.Where(tuple => tuple.Item2 == FileState.InTransferToRight).Select(tuple => tuple.Item1);


            return new SyncState(missingLeft.ToList(), missingRight.ToList(), conflicts.ToList(), Enumerable.Empty<string>(), inTransferToLeft, inTransferToRight);

        }


        private Tuple<String, FileState> ReadFileItem( XElement item)
        {
                string path = item.Attribute(XmlConstants.Path).Value;
                string typeStr = item.Attribute(XmlConstants.Type).Value;

                if(String.IsNullOrEmpty(path))
                {
                    throw new SyncStateException("Empty path found in item list");
                }
                FileState type;
                if(!Enum.TryParse<FileState>(typeStr, true, out type))
                {
                    throw new SyncStateException("Unknwon type: " + typeStr);
                }

                return new Tuple<string, FileState>(path, type);         
        }

    }
}
