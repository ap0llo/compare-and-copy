using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Model.Configuration;
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

        Logger m_Logger = LogManager.GetCurrentClassLogger();


        public override string Name
        {
            get { return "Copy"; }
        }

        public CopyAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder)
            :base(isEnabled, configuration, inputFilterName, syncFolder)
        {

        }


        public override void Run()
        {
            var sourceRoot = GetSyncFolderDefinition().RootPath;

            var targetRoot = this.SyncFolder == SyncFolder.Left ?
                                    Configuration.Right.RootPath :
                                    Configuration.Left.RootPath;

            foreach (var file in GetFilteredInput())
            {
                var absSource = Path.Combine(sourceRoot, file.RelativePath);
                var absTarget = Path.Combine(targetRoot, file.RelativePath);

                m_Logger.Info("Copying {0} to {1}", file.RelativePath, targetRoot);

                IOHelper.EnsureDirectoryExists(Path.GetDirectoryName(absTarget));
                File.Copy(absSource, absTarget, true);

                this.State.RemoveFile(file);
            }

        }
    }
}
