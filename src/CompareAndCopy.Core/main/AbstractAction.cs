using CompareAndCopy.Model.Actions;
using CompareAndCopy.Model.Configuration;
using CompareAndCopy.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompareAndCopy.Core
{
    /// <summary>
    /// Base implementation of <see cref="IAction"/>
    /// </summary>
    public abstract class AbstractAction : IAction
    {
        /// <summary>
        /// The action's name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets whether the Action is enabled
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        /// The configuration object to be used by the Action
        /// </summary>
        public ISyncConfiguration Configuration { get; }

        /// <summary>
        /// The current sync state (may be altered during execution of the action)
        /// </summary>
        public ISyncState State { get; set; }

        /// <summary>
        /// The name of the filter to apply to the input before processing it
        /// </summary>
        public string InputFilterName { get; }


        protected AbstractAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName)
        {
            IsEnabled = isEnabled;
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            InputFilterName = inputFilterName;
        }


        public abstract void Run();


        protected IEnumerable<IFileItem> GetFilteredInput()
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
    }
}
