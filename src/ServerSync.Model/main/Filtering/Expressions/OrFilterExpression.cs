using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter expression to implement a logical or.
    /// To be evaluated to true if any of the child expressions is evaluated to true
    /// </summary>
    public class OrFilterExpression : MultiFilterExpression
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of <see cref="OrFilterExpression"/>
        /// </summary>
        /// <param name="expressions">The expression's child expressions</param>
        public OrFilterExpression(params IFilterExpression[] expressions)
            : base(expressions)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="OrFilterExpression"/>
        /// </summary>
        /// <param name="expressions">The expression's child expressions</param>
        public OrFilterExpression(IEnumerable<IFilterExpression> expressions) : base(expressions)
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
