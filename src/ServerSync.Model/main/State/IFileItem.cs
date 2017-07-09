using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSync.Model.State
{
    /// <summary>
    /// Interface encapsulating a file to be processed during sync
    /// </summary>
    public interface IFileItem
    {
        /// <summary>
        /// The file's path relative to the sync folders root directory
        /// </summary>
        string RelativePath { get; }

        /// <summary>
        /// The current CompareState of the file
        /// </summary>
        CompareState CompareState { get; set; }

        /// <summary>
        /// The current TransferState of the file
        /// </summary>
        TransferState TransferState { get; }

    }
}
