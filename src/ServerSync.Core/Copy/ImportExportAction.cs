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
	/// <summary>
	/// Base class for both Import and Export action
	/// </summary>
	abstract class ImportExportAction : IOAction
	{
        #region Fields

        readonly IDictionary<string, ByteSize.ByteSize> m_TransferLocationSizeCache = new Dictionary<string, ByteSize.ByteSize>();

        #endregion

        #region Properties

        public string TransferLocationName { get; set; }

		public string TransferLocationSubPath { get; set; }
	   
        public bool AssumeExclusiveWriteAccess { get; set; }

        #endregion

        #region Constructor

        public ImportExportAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, 
								 SyncFolder syncFolder)
			: base(isEnabled, configuration, inputFilterName, syncFolder)
		{
			//if(transferLocationName == null)
			//{
			//    throw new ArgumentNullException("transferLocationName");
			//}

			//if(transferLocationName == null)
			//{
			//    throw new ArgumentNullException("transferLocationSubPath");
			//}

			//this.m_TransferLocationName = transferLocationName;
			//this.m_TransferLocationSubPath = transferLocationSubPath;
		}


        #endregion

        /// <summary>
        /// Checks whether copying a file of the specified size would exceed the maximum specified size for the transfer location
        /// </summary>
        protected bool CheckNextFileExceedsMaxTransferSize(ByteSize.ByteSize nextFileSize)
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

	    protected void UpdateTransferLocationSizeCache(ITransferLocation transferLocation, ByteSize.ByteSize fileSize)
	    {
            if (AssumeExclusiveWriteAccess)
            {
                m_TransferLocationSizeCache[transferLocation.RootPath] += fileSize;
            }
        }

    }
}