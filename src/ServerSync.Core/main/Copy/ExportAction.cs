using ServerSync.Model.Configuration;
using ServerSync.Model.State;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServerSync.Core.Copy
{
	/// <summary>
	/// Exports files from the current sync state to a specified location
	/// </summary>
	class ExportAction : ImportExportAction
	{        
		public override string Name => "Export";



	    public ExportAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder)
			: base(isEnabled, configuration, inputFilterName, syncFolder)
		{
		}

	

	    protected override IEnumerable<IFileItem> GetItemsToCopy()
	    {
            var direction = SyncFolder == SyncFolder.Left ?
                TransferDirection.InTransferToRight :
                TransferDirection.InTransferToLeft;

            return GetFilteredInput()
                .Where(item => item.TransferState.Direction != direction)
                .ToList();
        }

	    protected override string GetSourcePath(IFileItem item)
	    {
            return Path.Combine(GetSyncFolderDefinition().RootPath, item.RelativePath);
        }

	    protected override string GetTargetPath(IFileItem item)
	    {
            return Path.Combine(this.Configuration.GetTransferLocation(this.TransferLocationName).RootPath, this.TransferLocationSubPath, item.RelativePath);
        }

	    protected override void EnsurePathIsWithinSourceRoot(string path)
	    {
            var rootDir = GetSyncFolderDefinition().RootPath;
            if (IOHelper.PathLeavesRoot(rootDir, path))
            {
                throw new InvalidPathException($"Path '{path}' references file outside the root directory '{rootDir}'");
            }
        }

	    protected override void EnsurePathIsWithinTargetRoot(string path)
	    {
            var transferLocation = this.Configuration.GetTransferLocation(this.TransferLocationName);
            if (IOHelper.PathLeavesRoot(transferLocation.RootPath, path))
            {
                throw new InvalidPathException($"Path '{path}' references file outside root directory '{transferLocation.RootPath}'");
            }
        }    
	
	    protected override void OnItemCopied(IFileItem item)
	    {
            var newTransferDirection = SyncFolder == SyncFolder.Left ?
              TransferDirection.InTransferToRight :
              TransferDirection.InTransferToLeft;


            //set the item's new state
            item.TransferState.Direction = newTransferDirection;

            if (Flags.EnabledExtendedTransferState)
            {
                var transferLocation = this.Configuration.GetTransferLocation(this.TransferLocationName);
                item.TransferState.AddTransferLocation(transferLocation.RootPath);
            }
        }
	}
}
