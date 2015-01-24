using ServerSync.Model;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.State
{
    public class SyncStateWriter
    {

        /*
         *  SyncState files written by SyncStateWriter currently do not follow the SyncState xml schema to be compatible with older versions
         */

        #region Public Methods

        public void WriteSyncState(string fileName, SyncState state)
        {        
            XElement fileList = new XElement(XmlNames.FileList.LocalName, state.Files.Select(GetFileItemElement));

            var document = new XDocument(fileList);

            var dir = Path.GetDirectoryName(fileName);
            IOHelper.EnsureDirectoryExists(dir);

            document.Save(fileName);
        }

        #endregion 


        #region Private Implementation

        XElement GetFileItemElement(IFileItem file)
        {
            return new XElement( XmlNames.File.LocalName,
                                 new XAttribute(XmlAttributeNames.Path, file.RelativePath),
                                 new XAttribute(XmlAttributeNames.CompareState, file.CompareState),
                                 new XAttribute(XmlAttributeNames.TransferState, file.TransferState));
        }

        #endregion Private Implementation

    }
}
