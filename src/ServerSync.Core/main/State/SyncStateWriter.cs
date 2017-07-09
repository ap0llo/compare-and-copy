using ServerSync.Model.State;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ServerSync.Core.State
{
    public class SyncStateWriter
    {
        /*
         *  SyncState files written by SyncStateWriter currently do not conform with the SyncState xml schema to be compatible with older versions
         */

        public void WriteSyncState(string fileName, SyncState state)
        {        
            var fileList = new XElement(XmlNames.FileList.LocalName, state.Files.Select(GetFileItemElement));
            var document = new XDocument(fileList);

            var dir = Path.GetDirectoryName(fileName);
            IOHelper.EnsureDirectoryExists(dir);

            document.Save(fileName);
        }


        XElement GetFileItemElement(IFileItem file)
        {
            var locations = Flags.EnabledExtendedTransferState
                ? file.TransferState.Locations
                : Enumerable.Empty<string>();

            return new XElement(XmlNames.File.LocalName,
                                 new XAttribute(XmlAttributeNames.Path, file.RelativePath),
                                 new XAttribute(XmlAttributeNames.CompareState, file.CompareState),
                                 new XAttribute(XmlAttributeNames.TransferState, file.TransferState.Direction),
                                 locations.Select(path => new XElement(XmlNames.Location.LocalName, new XAttribute(XmlAttributeNames.Path, path))));
        }
    }
}
