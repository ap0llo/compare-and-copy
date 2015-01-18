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
    /// Filter element that filters based on the relative path of a <see cref="FileItem"/>
    /// </summary>
    public class RegexFilterExpression : IFilterExpression
    {
        
        #region Fields

        readonly  Regex regex;

        #endregion

        #region Properties

        public Regex Regex { get { return this.regex; } }

        #endregion

        #region Constructor

        public RegexFilterExpression(string pattern)
        {
            this.regex = new Regex(pattern);
        }

        #endregion


        #region IFilterEexpression Implementation

        public bool IsMatch(FileItem item)
        {
            return regex.IsMatch(item.RelativePath);
        }

        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion
    
    }
}
