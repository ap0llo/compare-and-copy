using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using ServerSync.Core.State;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;

namespace ServerSync.Core.Copy
{
    class ExportDirectoryAction : ImportExportAction
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        
        public override string Name => "ExportDirectory";

        public string SourcePath { get; set; }

        public bool DeleteSourceFiles { get; set; }

        
        public ExportDirectoryAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder) 
            : base(isEnabled, configuration, inputFilterName, syncFolder)
        {
        }

        
        public override void Run()
        {
            // save original state
            var syncState = State;

            // create temporary sync state
            var fileItems = IOHelper.GetAllFilesRelative(SourcePath)
                .Select(path => new FileItem(path, new TransferState()));
            State = new SyncState(fileItems);

            base.Run();

            // reset state to original value
            State = syncState;
        }


        protected override IEnumerable<IFileItem> GetItemsToCopy() => State.Files;

        protected override string GetSourcePath(IFileItem item) => Path.Combine(SourcePath, item.RelativePath);

        protected override string GetTargetPath(IFileItem item)
        {
            var rootPath = Configuration.GetTransferLocation(TransferLocationName).RootPath;
            return Path.Combine(rootPath, TransferLocationSubPath, item.RelativePath);
        }

        protected override void EnsurePathIsWithinSourceRoot(string path)
        {
            if (IOHelper.PathLeavesRoot(SourcePath, path))
            {
                throw new InvalidPathException($"Path '{path}' references file outside the root directory '{SourcePath}'");
            }
        }

        protected override void EnsurePathIsWithinTargetRoot(string path)
        {
            var transferLocation = Configuration.GetTransferLocation(TransferLocationName);
            if (IOHelper.PathLeavesRoot(transferLocation.RootPath, path))
            {
                throw new InvalidPathException($"Path '{path}' references file outside root directory '{transferLocation.RootPath}'");
            }
        }

        protected override void OnItemCopied(IFileItem item)
        {
            if (DeleteSourceFiles)
            {
                try
                {
                    File.Delete(GetSourcePath(item));
                }
                catch (IOException ex)
                {
                    m_Logger.Error($"Could not delete file '{item.RelativePath}'", ex);
                }
            }
        }
    }
}