using ServerSync.Model.State;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// A filter expression that filters based on the "CompareState" property of a <see cref="FileItem"/>
    /// To be evaluated to true for all FileItems which's CompareState is identical to the expression's compare state
    /// </summary>
    public class CompareStateFilterExpression : IFilterExpression
    {

        #region Fields

        readonly CompareState m_CompareState;

        #endregion


        #region Properties

        /// <summary>
        /// The CompareState to match
        /// </summary>
        public CompareState CompareState { get { return this.m_CompareState; } }

        #endregion


        #region Constructor

        public CompareStateFilterExpression(CompareState state)
        {
            this.m_CompareState = state;
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
