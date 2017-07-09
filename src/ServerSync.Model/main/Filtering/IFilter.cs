using ServerSync.Model.State;
using System.Collections.Generic;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Interface for a filter that may be used during a sync
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// The name of the filter
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Applies the filter to the specified list of files
        /// </summary>
        /// <param name="filterInput">The files to be filtered</param>
        /// <returns>Returns the files that match the criteria defined by the filter</returns>
        IEnumerable<IFileItem> ApplyFilter(IEnumerable<IFileItem> filterInput);
    }
}
