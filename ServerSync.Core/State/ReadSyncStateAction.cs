using ServerSync.Core.Configuration;

namespace ServerSync.Core.State
{
    class ReadSyncStateAction : AbstractAction
    {

        #region Properties


        public override string Name
        {
            get { return "ReadSyncState"; }
        }


        /// <summary>
        /// The file to read the state from
        /// </summary>
        public string FileName { get; set; }

        #endregion Properties


        #region Public Methods

        public override void Run()
        {
            var stateReader = new SyncStateReader();
            this.State = stateReader.ReadSyncState(this.FileName);
        }

        #endregion Public Methods
    
    }
}
