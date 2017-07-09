using ServerSync.Model.Configuration;
using System.Collections.Generic;
using System.IO;
using NLog;
using ServerSync.Model.State;

namespace ServerSync.Core.Copy
{
	/// <summary>
	/// Base class for both Import and Export action
	/// </summary>
	abstract class ImportExportAction : IOAction
	{        
	    readonly Logger m_Logger = LogManager.GetCurrentClassLogger();
        readonly IDictionary<string, ByteSize.ByteSize> m_TransferLocationSizeCache = new Dictionary<string, ByteSize.ByteSize>();

     
           
        public string TransferLocationName { get; set; }

		public string TransferLocationSubPath { get; set; }
	   
        public bool AssumeExclusiveWriteAccess { get; set; }

        
                
        protected ImportExportAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName,  SyncFolder syncFolder)
			: base(isEnabled, configuration, inputFilterName, syncFolder)
		{			
		}


        public override void Run()
        {            
            //determine all file times to copy
            var itemsToCopy = GetItemsToCopy();

            var transferLocation = Configuration.GetTransferLocation(TransferLocationName);
            if (transferLocation.MaximumSize.HasValue)
            {
                m_Logger.Info("Maximum size for transfer location: {0}", transferLocation.MaximumSize.Value.ToString("GB"));
            }


            foreach (var item in itemsToCopy)
            {
                //determine absolute paths for the copy operation
                var absSource = GetSourcePath(item);
                var absTarget = GetTargetPath(item);

                try
                {
                    EnsurePathIsWithinSourceRoot(absSource);
                    EnsurePathIsWithinTargetRoot(absTarget);
                }
                catch (PathTooLongException ex)
                {
                    m_Logger.Error($"Could not copy file '{item.RelativePath}': {ex.GetType().Name}");
                    continue;
                }

                //source file not found => skip file, write error to log
                if (!File.Exists(absSource))
                {
                    m_Logger.Error("File '{0}' could not be found", absSource);
                    continue;
                }

                var size = new FileInfo(absSource).GetByteSize();

                //check if copying the file would exceed the maximum transfer size
                //continue because there might be a file that can be copied without exceeding the max size
                //this way the copy as much as possible                 
                if (CheckNextFileExceedsMaxTransferSize(size))
                {
                    m_Logger.Info("Skipping '{0}' because copying it would exceed the maximum transfer size", item.RelativePath);
                    continue;
                }

                m_Logger.Info("Copying {0}", item.RelativePath);

                var success = FileEquals(absSource, absTarget) || IOHelper.CopyFile(absSource, absTarget); 

                if (success)
                {
                    UpdateTransferLocationSizeCache(transferLocation, size);
                    OnItemCopied(item);
                }

            }
        }

    

        protected abstract IEnumerable<IFileItem> GetItemsToCopy();

	    protected abstract string GetSourcePath(IFileItem item);

	    protected abstract string GetTargetPath(IFileItem item);

	    protected abstract void EnsurePathIsWithinSourceRoot(string path);

	    protected abstract void EnsurePathIsWithinTargetRoot(string path);

	    protected abstract void OnItemCopied(IFileItem item);



        /// <summary>
        /// Checks whether copying a file of the specified size would exceed the maximum specified size for the transfer location
        /// </summary>
        private bool CheckNextFileExceedsMaxTransferSize(ByteSize.ByteSize nextFileSize)
        {
            var transferLocation = Configuration.GetTransferLocation(this.TransferLocationName);

            // directory doesn't exist => limit not exceeded (no file copied yet)
            if (!Directory.Exists(transferLocation.RootPath))
            {
                return false;
            }

            if (AssumeExclusiveWriteAccess)
            {
                if (!m_TransferLocationSizeCache.ContainsKey(transferLocation.RootPath))
                {
                    m_TransferLocationSizeCache.Add(transferLocation.RootPath, IOHelper.GetDirectorySize(transferLocation.RootPath));
                }
            }

            //  maximum size for the transfer location itself has been specified
            if (transferLocation.MaximumSize.HasValue)
            {
                var currentSize = AssumeExclusiveWriteAccess
                    ? m_TransferLocationSizeCache[transferLocation.RootPath]
                    : IOHelper.GetDirectorySize(transferLocation.RootPath);

                //compare current size + file size + to maximum size
                return (currentSize + nextFileSize) > transferLocation.MaximumSize;
            }
            //  no maximum specified => no limit exceeded
            else
            {
                return false;
            }

        }

        private void UpdateTransferLocationSizeCache(ITransferLocation transferLocation, ByteSize.ByteSize fileSize)
        {
            if (AssumeExclusiveWriteAccess)
            {
                m_TransferLocationSizeCache[transferLocation.RootPath] += fileSize;
            }
        }

        private bool FileEquals(string path1, string path2)
        {
            var fileInfo1 = new FileInfo(path1);
            var fileInfo2 = new FileInfo(path2);

            if (!fileInfo1.Exists || !fileInfo2.Exists || fileInfo1.Exists != fileInfo2.Exists)
            {
                return false;
            }
            else
            {
                return fileInfo1.Length == fileInfo2.Length && fileInfo1.LastWriteTime == fileInfo2.LastWriteTime;
            }

        }


    }
}