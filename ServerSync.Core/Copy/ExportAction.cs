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
    class ExportAction : AbstractAction
    {

        #region Properties      

        
        public override string Name
        {
            get { return "Export"; }
        }


        /// <summary>
        /// The directory to export the files to
        /// </summary>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Specifies which sync directory to use as source for the export operation
        /// </summary>
        public Source Source { get; set; }

        #endregion Properties


        #region Public Method

        public override void Run()
        {
            //determine the root directory to copy the files from
            var rootDir = Source == Source.Left ? Configuration.Left.RootPath : Configuration.Right.RootPath;

            //determine the state to set for the items once they have been exported
            var newState = Source == Source.Left ? FileState.InTransferToLeft : FileState.InTransferToRight;

            //determine all file times to copy
            var copyItems = GetItemsToCopy();


            foreach (var item in copyItems)
            {
                //determine absolute paths for the copy operation
                var absSource = Path.Combine(rootDir, item.RelativePath);
                var absTarget = Path.Combine(TargetDirectory, item.RelativePath);

                Console.WriteLine("Copying {0}", item.RelativePath);

                //copy the file to the target
                EnsureDirectoryExists(Path.GetDirectoryName(absTarget));
                File.Copy(absSource, absTarget, true);

                //set the item's new state
                item.State = newState;
            }                       
        }

        #endregion


        #region Private Implementation

        private IEnumerable<FileItem> GetItemsToCopy()
        {
            return this.State.Files.Where(fileItem => fileItem.State == this.FileState);         
        }
      
        private void EnsureDirectoryExists(string path)
        {
            if(!Directory.Exists(path))
            {
                var parent = Path.GetDirectoryName(path);
                EnsureDirectoryExists(parent);

                Directory.CreateDirectory(path);
            }
        }

        #endregion Private Implementation
    }
}
