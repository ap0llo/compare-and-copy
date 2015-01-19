using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// A filter expression that filters based on an items TransferState property
    /// </summary>
    public class TransferStateFilterExpression : IFilterExpression
    {
        
        #region Fields

        readonly TransferState m_TransferState;

        #endregion


        #region Properties

        public TransferState TransferState { get { return this.m_TransferState; } }

        #endregion


        #region Constructor

        public TransferStateFilterExpression(TransferState state)
        {
            this.m_TransferState = state;
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
