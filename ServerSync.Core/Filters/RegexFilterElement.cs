using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class RegexFilterElement : IFilterElement
    {
        private Regex regex;


        public RegexFilterElement(string pattern)
        {
            this.regex = new Regex(pattern);
        }


        public bool IsMatch(FileItem item)
        {
            return regex.IsMatch(item.RelativePath);
        }
    }
}
