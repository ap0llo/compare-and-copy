using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Configuration
{
    /// <summary>
    /// Interface for the definition of a folder to be synced
    /// </summary>
    public interface ISyncFolderDefinition
    {
        /// <summary>
        /// The name of the folder
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The folder's root path
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// The role of the sync-folder in the sync (either left or right)
        /// </summary>
        SyncFolder Role { get; }
    }
}
