using System.IO;
using NLog;
using ServerSync.Model.Configuration;

namespace ServerSync.Core.Copy
{
    class CopyAction : IOAction
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();


        public override string Name => "Copy";


        public CopyAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder)
            :base(isEnabled, configuration, inputFilterName, syncFolder)
        {
        }


        public override void Run()
        {
            var sourceRoot = GetSyncFolderDefinition().RootPath;

            var targetRoot = SyncFolder == SyncFolder.Left 
                ? Configuration.Right.RootPath 
                : Configuration.Left.RootPath;

            foreach (var file in GetFilteredInput())
            {
                try
                {
                    if(IOHelper.PathLeavesRoot(sourceRoot, file.RelativePath))
                    {
                        throw new InvalidPathException($"Relative path '{file.RelativePath}' references file outside the root directory '{sourceRoot}'");                    
                    }

                    if (IOHelper.PathLeavesRoot(targetRoot, file.RelativePath))
                    {
                        throw new InvalidPathException($"Relative path '{file.RelativePath}' references file outside the root directory '{sourceRoot}'");
                    }
                }
                catch (PathTooLongException ex)
                {
                    m_Logger.Error($"Could not copy file '{file.RelativePath}': {ex.GetType().Name}");
                    continue;                    
                }

                var absSource = Path.Combine(sourceRoot, file.RelativePath);
                var absTarget = Path.Combine(targetRoot, file.RelativePath);

                m_Logger.Info("Copying {0} to {1}", file.RelativePath, targetRoot);


                var sourceInfo = new FileInfo(absSource);
                var targetInfo = new FileInfo(absTarget);

                if(!(sourceInfo.Exists && targetInfo.Exists && sourceInfo.LastWriteTime == targetInfo.LastWriteTime
                    && sourceInfo.Length == targetInfo.Length))
                {
                    IOHelper.CopyFile(absSource, absTarget);
                }                

                this.State.RemoveFile(file);
            }

        }
    }
}
