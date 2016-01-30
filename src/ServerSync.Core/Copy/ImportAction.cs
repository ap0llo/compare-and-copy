using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using ServerSync.Model;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
	class ImportAction : ImportExportAction
	{
		public override string Name => "Import";



	    public ImportAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder)
			: base(isEnabled, configuration, inputFilterName, syncFolder)
		{

		}


        protected override IEnumerable<IFileItem> GetItemsToCopy()
        {
            var direction = this.SyncFolder == SyncFolder.Left ?
                    TransferDirection.InTransferToLeft :
                    TransferDirection.InTransferToRight;

            return GetFilteredInput()
                    .Where(fileItem => fileItem.TransferState.Direction == direction)
                    .ToList();
        }

        protected override string GetSourcePath(IFileItem item)
	    {
            return Path.Combine(Configuration.GetTransferLocation(this.TransferLocationName).RootPath, this.TransferLocationSubPath, item.RelativePath);
        }

	    protected override string GetTargetPath(IFileItem item)
	    {
            var rootDir = GetSyncFolderDefinition().RootPath;
            return Path.Combine(rootDir, item.RelativePath);
        }

	    protected override void EnsurePathIsWithinSourceRoot(string path)
	    {
            var transferLocation = Configuration.GetTransferLocation(this.TransferLocationName);
            if (IOHelper.PathLeavesRoot(transferLocation.RootPath, path))
            {
                throw new InvalidPathException($"Path '{path}' references file outside the root directory '{transferLocation.RootPath}'");
            }
        }

	    protected override void EnsurePathIsWithinTargetRoot(string path)
	    {
            var rootDir = GetSyncFolderDefinition().RootPath;
            if (IOHelper.PathLeavesRoot(rootDir, path))
            {
                throw new InvalidPathException($"Path '{path}' references file outside the root directory '{rootDir}'");
            }
        }		

	    protected override void OnItemCopied(IFileItem item)
	    {
            State.RemoveFile(item);
        }

	}
}
