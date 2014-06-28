using ServerSync.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    abstract class IOAction : AbstractAction
    {

        public SyncFolder SyncFolder { get; set; }



        protected SyncFolderDefinition GetSyncFolderDefinition()
        {
            return this.SyncFolder == SyncFolder.Left ?
                this.Configuration.Left :
                this.Configuration.Right;
        }     


    }
}
