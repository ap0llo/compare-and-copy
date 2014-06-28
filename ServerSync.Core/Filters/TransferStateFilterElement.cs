using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class TransferStateFilterElement : IFilterElement
    {

        private TransferState state;


        public TransferStateFilterElement(TransferState state)
        {
            this.state = state;
        }


        public bool IsMatch(FileItem item)
        {
            return item.TransferState == this.state;
        }
    }
}
