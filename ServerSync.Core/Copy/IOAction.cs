using ServerSync.Core.Configuration;

namespace ServerSync.Core.Copy
{
    /// <summary>
    /// Base class for IO actions
    /// </summary>
    abstract class IOAction : AbstractAction
    {

        #region Properties

        public SyncFolder SyncFolder { get; set; }

        #endregion


        #region Protected Methods

        protected SyncFolderDefinition GetSyncFolderDefinition()
        {
            return this.SyncFolder == SyncFolder.Left ?
                this.Configuration.Left :
                this.Configuration.Right;
        }

        #endregion

    }
}
