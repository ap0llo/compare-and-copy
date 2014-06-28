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
