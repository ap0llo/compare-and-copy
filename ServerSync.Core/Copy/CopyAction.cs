using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    class CopyAction : IOAction
    {
        

        public override string Name
        {
            get { return "Copy"; }
        }


        public override void Run()
        {
            var sourceRoot = GetSyncFolderDefinition().RootPath;

            var targetRoot = this.SyncFolder == Copy.SyncFolder.Left ?
                                    Configuration.Right.RootPath :
                                    Configuration.Left.RootPath;

            foreach (var file in GetFilteredInput())
            {
                var absSource = Path.Combine(sourceRoot, file.RelativePath);
                var absTarget = Path.Combine(targetRoot, file.RelativePath);

                Console.WriteLine("Copying {0} to {1}", file.RelativePath, targetRoot);

                IOHelper.EnsureDirectoryExists(Path.GetDirectoryName(absTarget));
                File.Copy(absSource, absTarget, true);

                this.State.RemoveFile(file);
            }


        }
    }
}
