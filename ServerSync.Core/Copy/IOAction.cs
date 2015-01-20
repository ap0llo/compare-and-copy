using ServerSync.Core.Configuration;

namespace ServerSync.Core.Copy
{
    /// <summary>
    /// Base class for IO actions
    /// </summary>
    abstract class IOAction : AbstractAction
    {

        #region Fields

        readonly SyncFolder m_SyncFolder;

        #endregion


        #region Properties

        public SyncFolder SyncFolder { get { return m_SyncFolder; } }

        #endregion


        #region Constructor

        public IOAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder)
            : base(isEnabled, configuration, inputFilterName)
        {
            this.m_SyncFolder = syncFolder;
        }


        #endregion


        #region Protected Methods

        protected ISyncFolderDefinition GetSyncFolderDefinition()
        {
            return this.SyncFolder == SyncFolder.Left ?
                this.Configuration.Left :
                this.Configuration.Right;
        }

        #endregion

    }
}
