using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Compare
{
    class CompareAction : IAction
    {

        public bool IsEnabled { get; set; }

        public SyncConfiguration Configuration { get; set; }

        public SyncState State { get; set; }

        public void Run()
        {
            FolderComparer comparer = new FolderComparer(this.Configuration);
            var comparisonResult = comparer.Run();


            var combinedState = MergeSyncStates(this.State, comparisonResult);
            this.State = combinedState;
        }



        private static SyncState MergeSyncStates(SyncState exisiting, SyncState newSyncState)
        {
            var missingLeft = newSyncState.MissingLeft.Where(x => !exisiting.InTransferToLeft.Contains(x, StringComparer.InvariantCultureIgnoreCase));
            var missingRight = newSyncState.MissingRight.Where(x => !exisiting.InTransferToRight.Contains(x, StringComparer.InvariantCultureIgnoreCase));
            var conflicts = newSyncState.Conflicts.Where(x => !exisiting.InTransferToLeft.Contains(x, StringComparer.InvariantCultureIgnoreCase) &&
                                    !exisiting.InTransferToRight.Contains(x, StringComparer.InvariantCultureIgnoreCase));

            return new SyncState(missingLeft, missingRight, conflicts, newSyncState.SameFiles, exisiting.InTransferToLeft, exisiting.InTransferToRight);
        }


    }
}
