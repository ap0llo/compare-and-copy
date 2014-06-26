using ServerSync.Core.Configuration;

namespace ServerSync.Core.State
{
    class ReadSyncStateAction : IAction
    {

        #region Properties

        public bool IsEnabled { get; set; }

        public SyncConfiguration Configuration { get; set; }

        public SyncState State { get; set; }


        public string FileName { get; set; }

        #endregion Properties


        #region Public Methods

        public void Run()
        {
            var stateReader = new SyncStateReader();
            this.State = stateReader.ReadSyncState(this.FileName);
        }

        #endregion Public Methods
    
    }
}
