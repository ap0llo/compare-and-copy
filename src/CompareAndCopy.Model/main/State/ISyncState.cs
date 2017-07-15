using System.Collections.Generic;

namespace CompareAndCopy.Model.State
{
    /// <summary>
    /// Interface that encapsulates the entire "state" of a sync.  
    /// </summary>
    public interface ISyncState
    {
        /// <summary>
        /// The FileItems that are part of the state
        /// </summary>
        IEnumerable<IFileItem> Files { get; }

        /// <summary>
        /// Removes the specified FileItem from the state
        /// </summary>
        void RemoveFile(IFileItem item);        
    }
}
