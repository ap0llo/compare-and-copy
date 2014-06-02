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

        public IEnumerable<Regex> ExcludedFiles { get; set; }

        public IEnumerable<Regex> IncludeFolders { get; set; }

        public string LogDirectory { get; set; }

        public long TimeStampMargin { get; set; }


        public SyncConfiguration()
        {
            this.ExcludedFiles = Enumerable.Empty<Regex>();
            this.IncludeFolders = new List<Regex>(){ new Regex("[.]*") };
        }

    }
}
