using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter expression defining a logical not, negating another filter expression
    /// To be evaluated to true if the expression child expression evaluates to false
    /// </summary>
    public class NotFilterExpression : IFilterExpression
    {

        #region Fields

        readonly IFilterExpression m_NegatedExpression;

        #endregion


        #region Properties

        /// <summary>
        /// The expression which's result to negate
        /// </summary>
        public IFilterExpression NegatedExpression { get { return m_NegatedExpression; } }

        #endregion


        #region Constructor

        public NotFilterExpression(IFilterExpression negatedExpression)
        {
            if(negatedExpression == null)
            {
                throw new ArgumentNullException("negatedExpression");
            }

            this.m_NegatedExpression = negatedExpression;
        }

        #endregion


        #region IFilterExpression Implementation

        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}