using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter element that filters based on the "CompareState" property of a <see cref="FileItem"/>
    /// </summary>
    public class CompareStateFilterExpression : IFilterExpression
    {

        #region Fields

        readonly CompareState m_CompareState;

        #endregion


        #region Properties

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
