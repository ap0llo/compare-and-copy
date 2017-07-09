using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter expression describing a logical and over a variable number of child expressions.
    /// The expression is to be evaluated to true if all child expression evaluates to true
    /// </summary>
    public class AndFilterExpression : MultiFilterExpression
    {

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of <see cref="AndFilterExpression"/>
        /// </summary>
        /// <param name="expressions">The expression's child expressions</param>
        public AndFilterExpression(params  IFilterExpression[] expressions) : base(expressions)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AndFilterExpression"/>
        /// </summary>        
        /// <param name="expressions">The expression's child expressions</param>
        public AndFilterExpression(IEnumerable<IFilterExpression> expressions) : base(expressions)
        {
        }

        #endregion


        #region Overrides

        public override T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}
