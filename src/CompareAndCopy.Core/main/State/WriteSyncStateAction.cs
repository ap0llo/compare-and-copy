using CompareAndCopy.Model.Configuration;

namespace CompareAndCopy.Core.State
{
    class WriteSyncStateAction : AbstractAction
    {
        public override string Name => "WriteSyncState";

        /// <summary>
        /// The file to write the current state to
        /// </summary>
        public string FileName { get; }


        public WriteSyncStateAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, string fileName)
            : base(isEnabled, configuration, inputFilterName)
        {
            FileName = fileName;
        }


        public override void Run()
        {
            var writer = new SyncStateWriter();
            writer.WriteSyncState(this.FileName, new SyncState(GetFilteredInput()));
        }
    }
}
