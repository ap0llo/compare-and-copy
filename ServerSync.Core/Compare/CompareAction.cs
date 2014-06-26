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
            var filesExisitng = exisiting.Files.ToDictionary(fileItem => fileItem.RelativePath.Trim().ToLower());
            var filesNewState = exisiting.Files.ToDictionary(fileItem => fileItem.RelativePath.Trim().ToLower());

            foreach (var fileItem in newSyncState.Files.Where(x => x.State == FileState.MissingLeft))
            {
                var key = fileItem.RelativePath.ToLower().Trim();
                if(filesExisitng.ContainsKey(key) && filesExisitng[key].State == FileState.InTransferToLeft)
                {
                    fileItem.State = FileState.InTransferToLeft;
                }
            }

            foreach (var fileItem in newSyncState.Files.Where(x => x.State == FileState.MissingRight))
            {
                var key = fileItem.RelativePath.ToLower().Trim();
                if (filesExisitng.ContainsKey(key) && filesExisitng[key].State == FileState.InTransferToRight)
                {
                    fileItem.State = FileState.InTransferToRight;
                }
            }

            foreach (var fileItem in newSyncState.Files.Where(x => x.State == FileState.Conflict))
            {
                var key = fileItem.RelativePath.ToLower().Trim();
                if (filesExisitng.ContainsKey(key) && (filesExisitng[key].State == FileState.InTransferToRight || filesExisitng[key].State == FileState.InTransferToLeft))
                {
                    fileItem.State = filesExisitng[key].State;
                }
            }


            return newSyncState;
        }


    }
}
