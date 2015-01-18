using ServerSync.Core.Configuration;
using ServerSync.Core.Filters;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    /// <summary>
    /// Base implementation of <see cref="IAction"/>
    /// </summary>
    public abstract class AbstractAction : IAction
    {
        
        #region IAction Properties

        /// <summary>
        /// The action's name
        /// </summary>
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
        /// The name of the filter to apply to the input before processing it
        /// </summary>
        public string InputFilterName { get; set; }

        #endregion IAction Properties


        #region Public Methods

        public abstract void Run();

        #endregion


        #region Protected Members

        protected IEnumerable<FileItem> GetFilteredInput()
        {
            if(String.IsNullOrEmpty(this.InputFilterName))
            {
                return this.State.Files.ToList();
            }
            else
            {
                var filter = Configuration.GetFilter(this.InputFilterName);
                return filter.ApplyFilter(this.State.Files).ToList();
            }
        }

        #endregion

    }
}
