using CompareAndCopy.Model.State;

namespace CompareAndCopy.Model.Filtering
{
    /// <summary>
    /// A filter expression that filters based on the "CompareState" property of a <see cref="FileItem"/>
    /// To be evaluated to true for all FileItems which's CompareState is identical to the expression's compare state
    /// </summary>
    public class CompareStateFilterExpression : IFilterExpression
    {
        /// <summary>
        /// The CompareState to match
        /// </summary>
        public CompareState CompareState { get; }


        public CompareStateFilterExpression(CompareState state)
        {
            CompareState = state;
        }


        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter) => visitor.Visit(this, parameter);
    }
}
