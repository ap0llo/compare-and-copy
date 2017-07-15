using System;

namespace CompareAndCopy.Model.Filtering
{
    /// <summary>
    /// Filter expression defining a logical not, negating another filter expression
    /// To be evaluated to true if the expression child expression evaluates to false
    /// </summary>
    public class NotFilterExpression : IFilterExpression
    {
        /// <summary>
        /// The expression which's result to negate
        /// </summary>
        public IFilterExpression NegatedExpression { get; }


        public NotFilterExpression(IFilterExpression negatedExpression)
        {
            NegatedExpression = negatedExpression ?? throw new ArgumentNullException(nameof(negatedExpression));
        }

        
        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter) => visitor.Visit(this, parameter);
    }
}