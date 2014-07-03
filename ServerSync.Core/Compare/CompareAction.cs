using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Compare
{
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
            FolderComparer comparer = new FolderComparer(this.Configuration);
            var comparisonResult = comparer.Run();


            var combinedState = MergeSyncStates(this.State, comparisonResult);
            this.State = combinedState;
        }

        #endregion


        #region Private Implementation

        private SyncState MergeSyncStates(SyncState exisiting, SyncState newSyncState)
        {
            var filesExisitng = GetFilteredInput().ToDictionary(fileItem => fileItem.RelativePath.Trim().ToLower());           

            foreach (var fileItem in newSyncState.Files)
            {
                var key = fileItem.RelativePath.ToLower().Trim();
                if (filesExisitng.ContainsKey(key))
                {
                    fileItem.TransferState = filesExisitng[key].TransferState;
                }
            }
         

            return newSyncState;
        }

        #endregion

    }
}
