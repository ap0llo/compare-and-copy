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
        #region Fields

        readonly bool m_IsEnabled;
        readonly ISyncConfiguration m_Configuration;
        readonly string m_InputFilterName;

        #endregion


        #region IAction Properties

        /// <summary>
        /// The action's name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets whether the Action is enabled
        /// </summary>
        public bool IsEnabled { get { return m_IsEnabled; } }

        /// <summary>
        /// The configuration object to be used by the Action
        /// </summary>
        public ISyncConfiguration Configuration { get { return m_Configuration; } }

        /// <summary>
        /// The current sync state (may be altered during execution of the action)
        /// </summary>
        public SyncState State { get; set; }

        /// <summary>
        /// The name of the filter to apply to the input before processing it
        /// </summary>
        public string InputFilterName { get { return m_InputFilterName; } }

        #endregion IAction Properties


        #region Constructor

        public AbstractAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            
            this.m_IsEnabled = isEnabled;
            this.m_Configuration = configuration;
            this.m_InputFilterName = inputFilterName;
        }


        #endregion

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
