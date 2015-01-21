using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Linq;

namespace ServerSync.Core.Compare
{
    /// <summary>
    /// Implementation of the "Compare" Action
    /// </summary>
    class CompareAction : AbstractAction
    {

        #region Fields

        Logger m_Logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Properties

        public override string Name
        {
            get { return "Compare"; }
        }

        #endregion

        #region Constructor


        public CompareAction(bool isEnabled, ISyncConfiguration configuration)
            : base(isEnabled, configuration, null)
        {

        }

        #endregion


        #region Public Methods

        public override void Run()
        {
            //compare directories using FolderComparer
            FolderComparer comparer = new FolderComparer(this.Configuration);
            var comparisonResult = comparer.Run();

            if(comparisonResult == null)
            {
                m_Logger.Error("Error comparing folders");
                return; 
            }


            //combine new and old sync states
            var combinedState = MergeSyncStates(this.State, comparisonResult);
            this.State = combinedState;
        }

        #endregion


        #region Private Implementation

        /// <summary>
        /// Merges the exisitng SyncState into the new state.
        /// Merge is done by adding setting the TransferState for files that exist in both SyncStates to the value from "exisitingSyncState"
        /// </summary>
        /// <returns>Returns 'newSyncState'</returns>
        private ISyncState MergeSyncStates(ISyncState exisitingSyncState, ISyncState newSyncState)
        {
            //build dictionary with all files from existing sync state
            var filesExisting = exisitingSyncState.Files.ToDictionary(fileItem => fileItem.RelativePath.Trim().ToLower());           

            //iterate over all files from new sync state
            foreach (var fileItem in newSyncState.Files)
            {
                var key = fileItem.RelativePath.ToLower().Trim();

                //if file exists in both states, set TransferState to value from existing sync state
                if (filesExisting.ContainsKey(key))
                {
                    fileItem.TransferState = filesExisting[key].TransferState;
                }
            }         

            return newSyncState;
        }

        #endregion

    }
}
