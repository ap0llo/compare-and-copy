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
    class TransferStateFilterElement : IFilterElement
    {
        
        #region Fields

        private TransferState state;

        #endregion


        #region Constructor

        public TransferStateFilterElement(TransferState state)
        {
            this.state = state;
        }

        #endregion


        #region IFilterElement Implementation
        
        public bool IsMatch(FileItem item)
        {
            return item.TransferState == this.state;
        }

        #endregion

    }
}
