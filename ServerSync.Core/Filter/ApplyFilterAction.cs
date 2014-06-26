using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class ApplyFilterAction : IAction
    {
        #region Properties

        public bool IsEnabled { get; set; }

        public SyncConfiguration Configuration { get; set; }

        public SyncState State { get; set; }


        public string FilterName { get; set; }

        #endregion Properties


        #region Public Methods

        public void Run()
        {
            Filter filter = this.Configuration.GetFilter(this.FilterName);

            var filtered = this.State.Files.Where(file => filter.IncludeRules.Any(rule => rule.IsMatch(file.RelativePath)))
                                           .Where(file => !filter.ExcludeRules.Any(rule => rule.IsMatch(file.RelativePath)));

            this.State.Files = filtered.ToList();
            
        }

        #endregion Public Methods

    }
}
