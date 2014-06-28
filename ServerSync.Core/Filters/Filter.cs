using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    public class Filter
    {
        public string Name { get; set; }

        public IEnumerable<IFilterElement> IncludeRules { get; set; }

        public IEnumerable<IFilterElement> ExcludeRules { get; set; }


        public Filter()
        {
            IncludeRules = Enumerable.Empty<IFilterElement>();
            ExcludeRules = Enumerable.Empty<IFilterElement>();
        }

    }
}
