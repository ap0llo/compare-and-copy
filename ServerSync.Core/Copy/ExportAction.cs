﻿using ServerSync.Core.Compare;
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
                var size = new FileInfo(absSource).GetByteSize();                
               

                //check if copying the file would exceed the maximum tranfer size
                //continue because there might be a file thaht can be copied without exceeding the max size
                //this way the copy as much as possible                 
                if(CheckNextFileExceedsMaxTransferSize(size))
                {       
                    Console.WriteLine("Skipping '{0}' because copying it would exceed the maximum transfer size", item.RelativePath);
                    continue;                    
                }

                Console.WriteLine("Copying {0}", item.RelativePath);

                var success = IOHelper.CopyFile(absSource, absTarget);

                if(success)
                {
                    //set the item's new state
                    item.TransferState = newTransferState;                
                }

            }                       
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Checks whether copying a file of the specified size would exceed the maximum specified size for the transfer location
        /// </summary>
        private bool CheckNextFileExceedsMaxTransferSize(ByteSize.ByteSize nextFileSize)
        {
            // directory doesn't exist => limit not exceeded (no file copied yet)
            if (!Directory.Exists(TransferLocation))
            {
                return false;
            }

            //  maximum size for the transfer location itself has been specified
            if(MaxTransferSize.HasValue)
            {
                //get current size
                var currentSize = IOHelper.GetDirectorySize(TransferLocation);

                //compare current size + file size + to maximum size
                return (currentSize + nextFileSize) > MaxTransferSize;
            }
            // maximum size for the transfer location's parent directory has been specified
            else if(MaxTransferSizeParent.HasValue)
            {
                var parent = Path.GetDirectoryName(TransferLocation);
                
                //parent directory does not exist => no limit exceeded because no file has been copied yet
                if(!Directory.Exists(parent))
                {
                    return false;
                }

                //compare current size + file size + to maximum size
                var currentSize = IOHelper.GetDirectorySize(parent);

                return (currentSize + nextFileSize) > MaxTransferSizeParent;
            }
            //  no maximum specified => no limit exceeded
            else
            {
                return false;
            }

        }    

        #endregion

    }
}
