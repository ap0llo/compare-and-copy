using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class ApplyFilterAction : AbstractAction
    {
        #region Properties

        public override string Name
        {
            get { return "ApplyFilter"; }
        }

        public string FilterName { get; set; }

        #endregion Properties


        #region Public Methods

        public override void Run()
        {
            Filter filter = this.Configuration.GetFilter(this.FilterName);

            var filtered = this.State.Files.Where(file => filter.IncludeRules.Any(rule => rule.IsMatch(file)))
                                           .Where(file => !filter.ExcludeRules.Any(rule => rule.IsMatch(file)));

            this.State.Files = filtered.ToList();
            
        }

        #endregion Public Methods

    }
}
