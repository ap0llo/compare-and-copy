using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    class ImportAction : ImportExportAction
    {

        public override string Name
        {
            get { return "Import"; }
        }



        public override void Run()
        {
            var sourceTransferState = this.SyncFolder == SyncFolder.Left ?
                    TransferState.InTransferToLeft : 
                    TransferState.InTransferToRight;

            var targetRoot = GetSyncFolderDefinition().RootPath;

            

            var itemsToCopy = GetItemsToCopy(sourceTransferState);
            foreach (var file in itemsToCopy)
            {
                var absSource = Path.Combine(this.TransferLocation, file.RelativePath);
                var absTarget = Path.Combine(targetRoot, file.RelativePath);


                if(File.Exists(absSource))
                {
                    var dir = Path.GetDirectoryName(absTarget);
                    var size = new FileInfo(absSource).Length;

                    //check if copying the file would exceed the maximum tranfer size
                    //continue because there might be a file thaht can be copied without exceeding the max size
                    //this way the copy as much as possible 
                    if (MaxTransferSize.HasValue)
                    {
                        var transferSize = IOHelper.GetDirectorySize(TransferLocation);

                        if (transferSize.AddBytes(size) > MaxTransferSize)
                        {
                            Console.WriteLine("Skipping '{0}' because copying it would exceed the maximum transfer size", file.RelativePath);
                            continue;
                        }
                    }

                    IOHelper.EnsureDirectoryExists(dir);
                    File.Copy(absSource, absTarget);
                    State.RemoveFile(file);                   

                }
                else
                {
                    Console.WriteLine("ERROR: File not found: '{0}'", absSource);
                }
            }
         }



        private IEnumerable<FileItem> GetItemsToCopy(TransferState state)
        {
            return GetFilteredInput()
                    .Where(fileItem => fileItem.TransferState == state)
                    .ToList();
        }
    }
}
