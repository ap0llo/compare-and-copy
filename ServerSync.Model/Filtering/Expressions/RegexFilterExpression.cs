using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter expression that filters based on the relative path of a <see cref="FileItem"/> and a regular expression
    /// To be evaluated to true if a FileItem's relative path matches the expression's regular expression
    /// </summary>
    public class RegexFilterExpression : IFilterExpression
    {
        
        #region Fields

        readonly Regex m_Regex;

        #endregion


        #region Properties

        /// <summary>
        /// The regular expression to be used to evaluate a FileItem's relative path
        /// </summary>
        public Regex Regex { get { return this.m_Regex; } }

        #endregion


        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="RegexFilterExpression"/>
        /// </summary>
        /// <param name="pattern">The regular expression to be evaluated</param>
        public RegexFilterExpression(string pattern)
        {
            if(pattern == null)
            {
                throw new ArgumentNullException("pattern");
            }

            this.m_Regex = new Regex(pattern);
        }

        #endregion


        #region IFilterEexpression Implementation

        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion
    
    }
}
