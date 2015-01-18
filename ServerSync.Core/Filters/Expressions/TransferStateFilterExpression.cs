using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// A filter element that filters based on an items TransferState property
    /// </summary>
    public class TransferStateFilterExpression : IFilterExpression
    {
        
        #region Fields

        private TransferState state;

        #endregion


        #region Properties

        public TransferState TransferState { get { return this.state; } }

        #endregion

        #region Constructor

        public TransferStateFilterExpression(TransferState state)
        {
            this.state = state;
        }

        #endregion


        #region IFilterExpression Implementation
        
        public bool IsMatch(FileItem item)
        {
            return item.TransferState == this.state;
        }

        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}
