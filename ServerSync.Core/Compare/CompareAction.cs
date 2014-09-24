using ServerSync.Core.State;
using System.Linq;

namespace ServerSync.Core.Compare
{
    /// <summary>
    /// Implementation of the "Compare" Action
    /// </summary>
    class CompareAction : AbstractAction
    {

        #region Properties

        public override string Name
        {
            get { return "Compare"; }
        }

        #endregion


        #region Public Methods

        public override void Run()
        {
            //compare directoris using FolderComparer
            FolderComparer comparer = new FolderComparer(this.Configuration);
            var comparisonResult = comparer.Run();

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
        private SyncState MergeSyncStates(SyncState exisitingSyncState, SyncState newSyncState)
        {
            //build dictionary with all files from exisitng sync state
            var filesExisting = exisitingSyncState.Files.ToDictionary(fileItem => fileItem.RelativePath.Trim().ToLower());           

            //iterate over all files from new sync state
            foreach (var fileItem in newSyncState.Files)
            {
                var key = fileItem.RelativePath.ToLower().Trim();

                //if file exists in both states, set TransferState to value from exisitng sync state
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
