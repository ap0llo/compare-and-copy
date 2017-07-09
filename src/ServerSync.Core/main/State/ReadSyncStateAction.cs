using ServerSync.Model.Configuration;

namespace ServerSync.Core.State
{
    class ReadSyncStateAction : AbstractAction
    {
        public override string Name => "ReadSyncState";

        /// <summary>
        /// The file to read the state from
        /// </summary>
        public string FileName { get; }


        public ReadSyncStateAction(bool isEnabled, ISyncConfiguration configuration, string fileName)
            : base(isEnabled, configuration, null)
        {
            FileName = fileName;
        }


        public override void Run() => State = new SyncStateReader().ReadSyncState(this.FileName);
    }
}
