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
                    TransferState.InTransferToRight :
                    TransferState.InTransferToLeft;

            //determine all file times to copy
            var copyItems = GetFilteredInput();


            if(MaxTransferSize.HasValue)
            {
                Console.WriteLine("Copying at most {0} GB", MaxTransferSize.Value.ToString("GB"));
            }


           
            foreach (var item in copyItems)
            {
              
                if(item.TransferState == newTransferState)
                {
                    continue;
                }

                //determine absolute paths for the copy operation
                var absSource = Path.Combine(rootDir, item.RelativePath);
                var absTarget = Path.Combine(TransferLocation, item.RelativePath);
                var size = new FileInfo(absSource).Length;
                
                //check if copying the file would exceed the maximum tranfer size
                //continue because there might be a file thaht can be copied without exceeding the max size
                //this way the copy as much as possible                 
                if(MaxTransferSize.HasValue)
                {
                    var transferSize = IOHelper.GetDirectorySize(TransferLocation);

                    if(transferSize.AddBytes(size) > MaxTransferSize)
                    {
                        Console.WriteLine("Skipping '{0}' because copying it would exceed the maximum transfer size", item.RelativePath);
                        continue;
                    }
                }

                Console.WriteLine("Copying {0}", item.RelativePath);

                //copy the file to the target
                IOHelper.EnsureDirectoryExists(Path.GetDirectoryName(absTarget));

                var absTarget_tmp = absTarget + ".tmp";
                File.Copy(absSource, absTarget_tmp, true);

                File.Move(absTarget_tmp, absTarget);

                //set the item's new state
                item.TransferState = newTransferState;                

            }                       
        }

        #endregion





    }
}
