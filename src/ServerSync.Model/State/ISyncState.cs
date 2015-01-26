using ServerSync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSync.Model.State
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
