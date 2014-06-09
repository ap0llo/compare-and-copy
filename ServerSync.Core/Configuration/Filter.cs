using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration
{
    public class Filter
    {
        public IEnumerable<Regex> IncludeRules { get; set; }

        public IEnumerable<Regex> ExcludeRules { get; set; }


        public Filter()
        {
            IncludeRules = Enumerable.Empty<Regex>();
            ExcludeRules = Enumerable.Empty<Regex>();
        }

    }
}
