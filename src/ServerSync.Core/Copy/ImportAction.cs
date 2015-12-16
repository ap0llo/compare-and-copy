using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using ServerSync.Model;
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
	class ImportAction : ImportExportAction
	{

		Logger m_Logger = LogManager.GetCurrentClassLogger();


		public override string Name
		{
			get { return "Import"; }
		}


		#region Constructor
		
		public ImportAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, 
							SyncFolder syncFolder)
			: base(isEnabled, configuration, inputFilterName, syncFolder)
		{

		}

		#endregion

		public override void Run()
		{
			var sourceTransferState = this.SyncFolder == SyncFolder.Left ?
					TransferDirection.InTransferToLeft : 
					TransferDirection.InTransferToRight;

			var targetRoot = GetSyncFolderDefinition().RootPath;

			

			var itemsToCopy = GetItemsToCopy(sourceTransferState);
			foreach (var file in itemsToCopy)
			{
				var transferLocation = Configuration.GetTransferLocation(this.TransferLocationName);

				var absSource = Path.Combine(transferLocation.RootPath, this.TransferLocationSubPath, file.RelativePath);
				var absTarget = Path.Combine(targetRoot, file.RelativePath);

                if (IOHelper.PathLeavesRoot(transferLocation.RootPath, absSource))
                {
                    throw new InvalidPathException(String.Format("Path '{0}' references file outside the root directory '{1}'",
                        absSource, transferLocation.RootPath));
                }

                if (IOHelper.PathLeavesRoot(targetRoot, absTarget))
                {
                    throw new InvalidPathException(String.Format("Path '{0}' references file outside the root directory '{1}'",
                        absTarget, targetRoot));
                }



				if(File.Exists(absSource))
				{
					var size = new FileInfo(absSource).GetByteSize();

                    //check if copying the file would exceed the maximum transfer size
                    //continue because there might be a file that can be copied without exceeding the max size
                    //this way the copy as much as possible                 
                    if (CheckNextFileExceedsMaxTransferSize(size))
                    {
                        m_Logger.Info("Skipping '{0}' because copying it would exceed the maximum transfer size", file.RelativePath);
                        continue;
                    }
                    
					var success = IOHelper.CopyFile(absSource, absTarget);
				    if (success)
				    {
				        UpdateTransferLocationSizeCache(transferLocation, size);
				    }


					State.RemoveFile(file);
				}
				else
				{
					m_Logger.Info("File not found: '{0}'", absSource);
				}
			}
		 }



		private IEnumerable<IFileItem> GetItemsToCopy(TransferDirection direction)
		{
			return GetFilteredInput()
					.Where(fileItem => fileItem.TransferState.Direction == direction)
					.ToList();
        }



    }
}
