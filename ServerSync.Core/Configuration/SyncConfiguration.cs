using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration
{
    public class SyncConfiguration
    {
        public SyncFolder Left { get; set; }

        public SyncFolder Right { get; set; }

        public IEnumerable<Filter> Filters { get; set; }

        public string LogDirectory { get; set; }

        public long TimeStampMargin { get; set; }


        public IEnumerable<IAction> Actions { get; set; }

        public SyncConfiguration()
        {
            this.Filters = Enumerable.Empty<Filter>();
        }

    }
}
