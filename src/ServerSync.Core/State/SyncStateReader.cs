using ServerSync.Model;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace ServerSync.Core.State
{
    public class SyncStateReader
    {
        #region Constants

        const string s_SyncStateSchema = "ServerSync.Core.State.SyncStateSchema.xsd";

        #endregion


        #region Public Methods

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

            document.Validate(GetSyncStateSchema(), (o, e) => { throw new SyncStateException(e.Message); });



            var files = document.Descendants(XmlNames.File).Select(ReadFileItem);

            return new SyncState(files.ToList());
        }

        #endregion


        #region Private Implementation

        IFileItem ReadFileItem( XElement item)
        {
            string path = item.Attribute(XmlAttributeNames.Path).Value;

            if(String.IsNullOrEmpty(path))
            {
                throw new SyncStateException("Empty path found in item list");
            }

            string compareStateStr = item.RequireAttributeValue(XmlAttributeNames.CompareState);
            CompareState compareState;
            if(!Enum.TryParse<CompareState>(compareStateStr, true, out compareState))
            {
                throw new SyncStateException("Unknown type: " + compareStateStr);
            }

            var transferStateStr = item.RequireAttributeValue(XmlAttributeNames.TransferState);
            TransferDirection transferState;
            if(!Enum.TryParse<TransferDirection>(transferStateStr, out transferState))
            {
                throw new SyncStateException("Unknown type: " + transferStateStr);
            }

            return new FileItem(path) { CompareState = compareState, TransferState = new TransferState(transferState) };
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


        #endregion

    }
}
