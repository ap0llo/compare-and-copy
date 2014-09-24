using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// Filter element taht filters based on the relative path of a <see cref="FileItem"/>
    /// </summary>
    class RegexFilterElement : IFilterElement
    {
        
        #region Fields

        private Regex regex;

        #endregion


        #region Constructor

        public RegexFilterElement(string pattern)
        {
            this.regex = new Regex(pattern);
        }

        #endregion


        #region IFilterElement Implementation

        public bool IsMatch(FileItem item)
        {
            return regex.IsMatch(item.RelativePath);
        }

        #endregion
    
    }
}
