using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.State
{

    /// <summary>
    /// Defines the values that describe the transfer state of a file
    /// A state other than none indicates that the file is currently being transfered to the respectively other sync folder
    /// and thus should not be copied again
    /// </summary>
    public enum TransferDirection
    {
        None = 0,
        InTransferToLeft = 1,
        InTransferToRight = 2
    }
}
