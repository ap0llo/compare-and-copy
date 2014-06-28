using ServerSync.Core.Configuration;
using ServerSync.Core.Copy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    class WriteSyncStateAction : AbstractAction
    {

        #region Properties


        public override string Name
        {
            get { return "WriteSyncState"; }
        }


        /// <summary>
        /// The file to write the current state to
        /// </summary>
        public string FileName { get; set; }

        #endregion


        #region Public Methods

        public override void Run()
        {
            var writer = new SyncStateWriter();
            writer.WriteSyncState(this.FileName, new SyncState(GetFilteredInput()));
        }

        #endregion
    }
}
