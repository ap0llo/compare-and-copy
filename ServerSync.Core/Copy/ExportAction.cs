using ServerSync.Core.Compare;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    /// <summary>
    /// Exports files from the current sync state to a specified location
    /// </summary>
    class ExportAction : ImportExportAction
    {

        #region Properties

        
        public override string Name
        {
            get { return "Export"; }
        }


        #endregion Properties


        #region Public Method

        public override void Run()
        {
            //determine the root directory to copy the files from
            var rootDir = GetSyncFolderDefinition().RootPath;

            //determine the state to set for the items once they have been exported
            var newTransferState = SyncFolder == SyncFolder.Left ? 
                    TransferState.InTransferToLeft : 
                    TransferState.InTransferToRight;

            //determine all file times to copy
            var copyItems = GetFilteredInput();


            foreach (var item in copyItems)
            {
                //determine absolute paths for the copy operation
                var absSource = Path.Combine(rootDir, item.RelativePath);
                var absTarget = Path.Combine(TransferLocation, item.RelativePath);

                Console.WriteLine("Copying {0}", item.RelativePath);

                //copy the file to the target
                EnsureDirectoryExists(Path.GetDirectoryName(absTarget));
                File.Copy(absSource, absTarget, true);

                //set the item's new state
                item.TransferState = newTransferState;
            }                       
        }

        #endregion


    }
}
