using ServerSync.Core.Configuration;
using ServerSync.Core.Copy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    class ReadSyncStateAction : IAction
    {

        public bool IsEnabled { get; set; }

        public SyncConfiguration Configuration { get; set; }

        public SyncState State { get; set; }


        public string FileName { get; set; }



        public void Run()
        {

            var stateReader = new SyncStateReader();
            this.State = stateReader.ReadSyncState(this.FileName);

        }

    }
}
