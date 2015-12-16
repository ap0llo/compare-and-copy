using NLog;
using ServerSync.Core.Compare;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;
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
	class ExportAction : ImportExportAction
	{

		#region Fields

		readonly Logger m_Logger = LogManager.GetCurrentClassLogger();         

		#endregion


		#region Properties

		public override string Name
		{
			get { return "Export"; }
		}


		#endregion Properties


		#region Constructor
		
		public ExportAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, SyncFolder syncFolder)
			: base(isEnabled, configuration, inputFilterName, syncFolder)
		{

		}

		#endregion


		#region Public Method

		public override void Run()
		{
			//determine the root directory to copy the files from
			var rootDir = GetSyncFolderDefinition().RootPath;

			//determine the state to set for the items once they have been exported
			var newTransferDirection = SyncFolder == SyncFolder.Left ?
					TransferDirection.InTransferToRight :
					TransferDirection.InTransferToLeft;

			//determine all file times to copy
			var copyItems = GetFilteredInput();

			var transferLocation = this.Configuration.GetTransferLocation(this.TransferLocationName);

			if(transferLocation.MaximumSize.HasValue)
			{
				m_Logger.Info("Maximum size for transfer location: {0}", transferLocation.MaximumSize.Value.ToString("GB"));
			}

		   
			foreach (var item in copyItems)
			{			  
				if(item.TransferState.Direction == newTransferDirection)
				{
					continue;
				}


				//determine absolute paths for the copy operation
				var absSource = Path.Combine(rootDir, item.RelativePath);

                if (IOHelper.PathLeavesRoot(rootDir, absSource))
				{
					throw new InvalidPathException(String.Format("Path '{0}' references file outside the root directory '{1}'",
                        absSource, rootDir));
				}

				//source file not found => skip file, write error to log
				if(!File.Exists(absSource))
				{
					m_Logger.Error("File '{0}' could not be found", absSource);
					continue;
				}

		 
				var absTarget = Path.Combine(transferLocation.RootPath, this.TransferLocationSubPath, item.RelativePath);
		
				if(IOHelper.PathLeavesRoot(transferLocation.RootPath, absTarget))
				{
					throw new InvalidPathException(String.Format("Path '{0}' references file outside root directory '{1}'",
						absTarget, transferLocation.RootPath));
				}
				
				var size = new FileInfo(absSource).GetByteSize();                
			   

				//check if copying the file would exceed the maximum transfer size
				//continue because there might be a file that can be copied without exceeding the max size
				//this way the copy as much as possible                 
				if(CheckNextFileExceedsMaxTransferSize(size))
				{       
					m_Logger.Info("Skipping '{0}' because copying it would exceed the maximum transfer size", item.RelativePath);
					continue;                    
				}

				m_Logger.Info("Copying {0}", item.RelativePath);


				bool success;

				//check if file has already been copied (TransferState might be wrong)
				var sourceFileInfo = new FileInfo(absSource);
				var targetFileInfo = new FileInfo(absTarget);
				if (targetFileInfo.Exists && sourceFileInfo.Length == targetFileInfo.Length && targetFileInfo.LastWriteTime == sourceFileInfo.LastWriteTime)
				{
					success = true;
				}
				else
				{
					success = IOHelper.CopyFile(absSource, absTarget);

				    if (success )
				    {
				        UpdateTransferLocationSizeCache(transferLocation, size);
				    }
				}


				if (success)
				{
					//set the item's new state
					item.TransferState.Direction = newTransferDirection;
                    item.TransferState.AddTransferLocation(transferLocation.RootPath);                			        
				}

			}                       
		}

		#endregion


		#region Private Methods

		
	   

		#endregion

	}
}
