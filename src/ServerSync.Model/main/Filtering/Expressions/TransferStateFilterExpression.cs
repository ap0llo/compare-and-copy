using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// A filter expression that filters based on the "TransferState" property of a <see cref="FileItem"/>
    /// To be evaluated to true for all FileItems which's TransferState is identical to the expression's compare state
    /// </summary>
    public class TransferStateFilterExpression : IFilterExpression
    {
        
        #region Fields

        readonly TransferDirection m_TransferState;

        #endregion


        #region Properties

        /// <summary>
        /// The TransferState to match
        /// </summary>
        public TransferDirection TransferState { get { return this.m_TransferState; } }

        #endregion


        #region Constructor

        public TransferStateFilterExpression(TransferDirection state)
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
