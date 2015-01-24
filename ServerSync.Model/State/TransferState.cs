using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.State
{
    public enum TransferState
    {
        None = 0,
        InTransferToLeft = 1,
        InTransferToRight = 2
    }
}
