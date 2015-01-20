using NLog;
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
	class ImportAction : ImportExportAction
	{

		Logger m_logger = LogManager.GetCurrentClassLogger();


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
					TransferState.InTransferToLeft : 
					TransferState.InTransferToRight;

			var targetRoot = GetSyncFolderDefinition().RootPath;

			

			var itemsToCopy = GetItemsToCopy(sourceTransferState);
			foreach (var file in itemsToCopy)
			{
				var transferLocation = Configuration.GetTransferLocation(this.TransferLocationName);

				var absSource = Path.Combine(transferLocation.Path, this.TransferLocationSubPath, file.RelativePath);
				var absTarget = Path.Combine(targetRoot, file.RelativePath);


				if(File.Exists(absSource))
				{
					var dir = Path.GetDirectoryName(absTarget);
					var size = new FileInfo(absSource).Length;

					//check if copying the file would exceed the maximum transfer size
					//continue because there might be a file that can be copied without exceeding the max size
					//this way the copy as much as possible                     
					if (transferLocation.MaximumSize.HasValue)
					{
						var transferSize = IOHelper.GetDirectorySize(transferLocation.Path);

						if (transferSize.AddBytes(size) > transferLocation.MaximumSize)
						{
							m_logger.Info("Skipping '{0}' because copying it would exceed the maximum transfer size", file.RelativePath);
							continue;
						}
					}

					IOHelper.EnsureDirectoryExists(dir);
					File.Copy(absSource, absTarget);
					State.RemoveFile(file);
				}
				else
				{
					m_logger.Info("File not found: '{0}'", absSource);
				}
			}
		 }



		private IEnumerable<FileItem> GetItemsToCopy(TransferState state)
		{
			return GetFilteredInput()
					.Where(fileItem => fileItem.TransferState == state)
					.ToList();
		}
	}
}
