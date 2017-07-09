using System.Collections.Generic;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter expression to implement a logical or.
    /// To be evaluated to true if any of the child expressions is evaluated to true
    /// </summary>
    public class OrFilterExpression : MultiFilterExpression
    {
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

        
        public override T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter) => visitor.Visit(this, parameter);
    }
}
