using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.State
{
    public class SyncStateWriter
    {
        public void WriteSyncState(string fileName, SyncState result)
        {
            var missingLeft = result.MissingLeft.Select(path => GetFileNode(path, FileState.MissingLeft));
            var missingRight = result.MissingRight.Select(path => GetFileNode(path, FileState.MissingRight));
            var conflicts = result.Conflicts.Select(path => GetFileNode(path, FileState.Conflict));
            var inTransferToLeft = result.InTransferToLeft.Select(path => GetFileNode(path, FileState.InTransferToLeft));
            var inTransferToRight = result.InTransferToRight.Select(path => GetFileNode(path, FileState.InTransferToRight));

            XElement fileList = new XElement(XmlConstants.FileList, missingLeft.Union(missingRight).Union(conflicts).Union(inTransferToLeft).Union(inTransferToRight));

            var document = new XDocument(new XElement(fileList));
            document.Save(fileName);
        }



        private XElement GetFileNode(string path, FileState type)
        {
            return new XElement( XmlConstants.File, 
                                 new XAttribute(XmlConstants.Path, path), 
                                 new XAttribute(XmlConstants.Type, type) );
        }


    }
}
