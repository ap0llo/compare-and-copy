using ServerSync.Core.Configuration;
using ServerSync.Core.Copy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    class WriteSyncStateAction : IAction
    {

        #region Properties

        public bool IsEnabled { get; set; }

        public SyncConfiguration Configuration { get; set; }

        public SyncState State { get; set; }

        public string FileName { get; set; }

        #endregion


        #region Public Methods

        public void Run()
        {
            var writer = new SyncStateWriter();
            writer.WriteSyncState(this.FileName, this.State);

        }

        #endregion
    }
}
