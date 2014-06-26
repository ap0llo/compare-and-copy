using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    public abstract class AbstractAction : IAction
    {
        
        #region IAction Properties

        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets whether the Action is enabled
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The configuration object to be used by the Action
        /// </summary>
        public SyncConfiguration Configuration { get; set; }

        /// <summary>
        /// The current sync state (may be altered during execution of the action)
        /// </summary>
        public SyncState State { get; set; }

        /// <summary>
        /// Specifies the state of files to include in the operation
        /// </summary>
        public FileState FileState { get; set; }

        #endregion IAction Properties


        #region Public Methods

        public abstract void Run();

        #endregion

    }
}
