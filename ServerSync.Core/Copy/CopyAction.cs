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
    class CopyAction : IAction
    {



        public bool IsEnabled { get; set; }

        public SyncConfiguration Configuration { get; set; }

        public SyncState State { get; set; }


        public FileState ItemType { get; set; }
        public FileState SetStateTo { get; set; }

        public string TargetDirectory { get; set; }

        public Source Source { get; set; }


        public void Run()
        {
            var root = Source == Source.Left ? Configuration.Left.RootPath : Configuration.Right.RootPath;

            var copyItems = GetItemsToCopy();
            foreach (var item in copyItems)
            {
                var absSource = Path.Combine(root, item.RelativePath);
                var absTarget = Path.Combine(TargetDirectory, item.RelativePath);

                Console.WriteLine("Copying {0}", item.RelativePath);

                EnsureDirectoryExists(Path.GetDirectoryName(absTarget));
                File.Copy(absSource, absTarget);
                item.State = SetStateTo;
            }                       
        }


        private IEnumerable<FileItem> GetItemsToCopy()
        {
            return this.State.Files.Where(fileItem => fileItem.State == this.ItemType);         
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

    }
}
