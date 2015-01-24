using ServerSync.Model.Actions;
using ServerSync.Model.Filtering;
using System;
using System.Collections.Generic;

namespace ServerSync.Model.Configuration
{
    /// <summary>
    /// Encapsulates all configuration necessary to execute a sync job
    /// </summary>
    public interface ISyncConfiguration
    {

        #region Properties

        /// <summary>
        /// The "left" of the folders to be synced
        /// </summary>
        ISyncFolderDefinition Left { get; set; }

        /// <summary>
        /// The "right" of the folders to be synced
        /// </summary>
        ISyncFolderDefinition Right { get; set; }

        /// <summary>
        /// The margin by which timestamps of files may differ while still being regarded as identical
        /// </summary>
        TimeSpan TimeStampMargin { get; set; }

        /// <summary>
        /// All filters defined for the sync job
        /// </summary>
        IEnumerable<IFilter> Filters { get;}
        
        /// <summary>
        /// All actions to be executed during sync
        /// </summary>
        IEnumerable<IAction> Actions { get;}

        /// <summary>
        /// All transfer locations defined for the sync job
        /// </summary>
        IEnumerable<ITransferLocation> TransferLocations { get; }

        #endregion 


        #region Public Methods

        /// <summary>
        /// Adds a filter to the sync configuration
        /// </summary>
        /// <param name="filter">The filter to be added to the sync configuration</param>
        void AddFilter(IFilter filter);
        
        /// <summary>
        /// Gets a filter from the sync configuration
        /// </summary>
        /// <param name="name">The name of the filter to search for</param>        
        IFilter GetFilter(string name);
        
        /// <summary>
        /// Adds a action to the sync configuration
        /// </summary>
        /// <param name="action">The action to be added to the sync configuration</param>
        void AddAction(IAction action);

        /// <summary>
        /// Adds the definition of a transfer location to the sync configuration
        /// </summary>
        /// <param name="transferLocation">The transfer location to be added to the sync configuration</param>
        void AddTransferLocation(ITransferLocation transferLocation);
        
        /// <summary>
        /// Gets a transfer location from the sync configuration
        /// </summary>
        /// <param name="name">The name of the transfer location to search for</param>        
        ITransferLocation GetTransferLocation(string name);       

        #endregion

    }
}
