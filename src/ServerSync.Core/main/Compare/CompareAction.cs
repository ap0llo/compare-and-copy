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
	    readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        
		public override string Name => "Compare";

	    public TimeSpan TimeStampMargin { get; }
        

	    public CompareAction(bool isEnabled, string inputFilterName, ISyncConfiguration configuration, TimeSpan timeStampMargin)
			: base(isEnabled, configuration, inputFilterName)
		{
			TimeStampMargin = timeStampMargin;
		}


		public override void Run()
		{
			//compare directories using FolderComparer
			var comparer = new FolderComparer(this.Configuration, this.TimeStampMargin);
			var comparisonResult = comparer.Run();

			if(comparisonResult == null)
			{
				m_Logger.Error("Error comparing folders");
				return; 
			}


			//combine new and old sync states
			var combinedState = MergeSyncStates(new SyncState(GetFilteredInput()), comparisonResult);
			State = combinedState;
		}


		/// <summary>
		/// Merges the existing SyncState into the new state.
		/// Merge is done by adding setting the TransferState for files that exist in both SyncStates to the value from "exisitingSyncState"
		/// </summary>
		/// <returns>Returns 'newSyncState'</returns>
		ISyncState MergeSyncStates(ISyncState exisitingSyncState, ISyncState newSyncState)
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
	}
}
