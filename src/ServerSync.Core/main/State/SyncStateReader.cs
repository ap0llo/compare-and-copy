using ServerSync.Model.State;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ServerSync.Core.State
{
    public class SyncStateReader
    {
        const string s_SyncStateSchema = "ServerSync.Core.State.SyncStateSchema.xsd";


        public SyncState ReadSyncState(string fileName)
        {
            if(!File.Exists(fileName))
            {
                return new SyncState();
            }

            XDocument document;
            try
            {
                document = XDocument.Load(fileName);
            }
            catch (XmlException)
            {
                throw new JobExecutionException("Error reading the sync state file. File is not valid xml");
            }

            if(String.IsNullOrEmpty(document.Root.Name.NamespaceName))
            {
                document.Root.ReplaceNamespace("", XmlNames.GetNamespace());
            }

            document.Validate(GetSyncStateSchema(), (o, e) => throw new SyncStateException(e.Message));

            var files = document.Descendants(XmlNames.File).Select(ReadFileItem);
            return new SyncState(files.ToList());
        }


        IFileItem ReadFileItem(XElement item)
        {
            var path = item.Attribute(XmlAttributeNames.Path).Value;

            if(String.IsNullOrEmpty(path))
            {
                throw new SyncStateException("Empty path found in item list");
            }

            var compareStateStr = item.RequireAttributeValue(XmlAttributeNames.CompareState);
            if (!Enum.TryParse(compareStateStr, true, out CompareState compareState))
            {
                throw new SyncStateException("Unknown type: " + compareStateStr);
            }

            var transferStateStr = item.RequireAttributeValue(XmlAttributeNames.TransferState);
            if (!Enum.TryParse(transferStateStr, out TransferDirection transferDirection))
            {
                throw new SyncStateException("Unknown type: " + transferStateStr);
            }

            var locations = Flags.EnabledExtendedTransferState
                ? item.Elements(XmlNames.Location).Select(xml => xml.RequireAttributeValue(XmlAttributeNames.Path))
                : Enumerable.Empty<string>();

            return new FileItem(path, new TransferState(transferDirection, locations)) { CompareState = compareState };
        }

        XmlSchemaSet GetSyncStateSchema()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(s_SyncStateSchema))
            {
                var schemaSet = new XmlSchemaSet();
                schemaSet.Add(null, XmlReader.Create(stream));
                return schemaSet;
            }
        }
    }
}
