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

		//readonly string m_TransferLocationName;
		//readonly string m_TransferLocationSubPath;

		#endregion

		#region Properties

		public string TransferLocationName { get; set; }

		public string TransferLocationSubPath { get; set;}
	   
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


	}
}