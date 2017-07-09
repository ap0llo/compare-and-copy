using NLog;
using ServerSync.Core.State;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;
using System;
using System.Linq;

namespace ServerSync.Core.Compare
{
	/// <summary>
	/// Implementation of the "Compare" Action
	/// </summary>
	class CompareAction : AbstractAction
	{

		#region Fields

		readonly TimeSpan m_TimeStampMargin;
		readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

		#endregion


		#region Properties

		public override string Name
		{
			get { return "Compare"; }
		}

		public TimeSpan TimeStampMargin { get { return m_TimeStampMargin; } }

		#endregion

		#region Constructor


		public CompareAction(bool isEnabled, string inputFilterName, ISyncConfiguration configuration, TimeSpan timeStampMargin)
			: base(isEnabled, configuration, inputFilterName)
		{
			this.m_TimeStampMargin = timeStampMargin;
		}

		#endregion


		#region Public Methods

		public override void Run()
		{
			//compare directories using FolderComparer
			FolderComparer comparer = new FolderComparer(this.Configuration, this.TimeStampMargin);
			var comparisonResult = comparer.Run();

			if(comparisonResult == null)
			{
				m_Logger.Error("Error comparing folders");
				return; 
			}


			//combine new and old sync states
			var combinedState = MergeSyncStates(new SyncState(GetFilteredInput()), comparisonResult);
			this.State = combinedState;
		}

		#endregion


		#region Private Implementation

		/// <summary>
		/// Merges the existing SyncState into the new state.
		/// Merge is done by adding setting the TransferState for files that exist in both SyncStates to the value from "exisitingSyncState"
		/// </summary>
		/// <returns>Returns 'newSyncState'</returns>
		private ISyncState MergeSyncStates(ISyncState exisitingSyncState, ISyncState newSyncState)
		{
			//build dictionary with all files from existing sync state
			var filesExisting = exisitingSyncState.Files.ToDictionary(fileItem => fileItem.RelativePath.Trim().ToLower());           

			//iterate over all files from new sync state
			foreach (var fileItem in newSyncState.Files)
			{
				var key = fileItem.RelativePath.ToLower().Trim();

				//if file exists in both states, set TransferState to value from existing sync state
				if (filesExisting.ContainsKey(key))
				{
					fileItem.TransferState.Direction = filesExisting[key].TransferState.Direction;
				    if (Flags.EnabledExtendedTransferState)
				    {
                        fileItem.TransferState.Locations = filesExisting[key].TransferState.Locations;				        
				    }

                }
			}         

			return newSyncState;
		}

		#endregion

	}
}
