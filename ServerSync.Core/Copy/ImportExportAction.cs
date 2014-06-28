using ServerSync.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    abstract class ImportExportAction : AbstractAction
    {

        #region Properties

        public SyncFolder SyncFolder { get; set; }

        public string TransferLocation { get; set; }

        #endregion


        protected SyncFolderDefinition GetSyncFolderDefinition()
        {
            return this.SyncFolder == SyncFolder.Left ?
                this.Configuration.Left :
                this.Configuration.Right;
        }


        protected void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                var parent = Path.GetDirectoryName(path);
                EnsureDirectoryExists(parent);

                Directory.CreateDirectory(path);
            }
        }

    }
}