using ServerSync.Model.Configuration;

namespace ServerSync.Core.Copy
{
    /// <summary>
    /// Base class for IO actions
    /// </summary>
    abstract class IOAction : AbstractAction
    {
        public SyncFolder SyncFolder { get; }


        protected IOAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder)
            : base(isEnabled, configuration, inputFilterName)
        {
            SyncFolder = syncFolder;
        }


        protected ISyncFolderDefinition GetSyncFolderDefinition()
        {
            return SyncFolder != SyncFolder.Left 
                ? Configuration.Right 
                : Configuration.Left;
        }
    }
}
