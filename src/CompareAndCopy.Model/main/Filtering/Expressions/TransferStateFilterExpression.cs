using CompareAndCopy.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareAndCopy.Model.Filtering
{
    /// <summary>
    /// A filter expression that filters based on the "TransferState" property of a <see cref="FileItem"/>
    /// To be evaluated to true for all FileItems which's TransferState is identical to the expression's compare state
    /// </summary>
    public class TransferStateFilterExpression : IFilterExpression
    {
        /// <summary>
        /// The TransferState to match
        /// </summary>
        public TransferDirection TransferState { get; }


        public TransferStateFilterExpression(TransferDirection state)
        {
            TransferState = state;
        }


        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter) => visitor.Visit(this, parameter);
    }
}
