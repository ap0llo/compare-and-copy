using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSync.Model.State
{
    public interface IFileItem
    {

        string RelativePath { get; set; }

        CompareState CompareState { get; set; }

        TransferState TransferState { get; set; }

    }
}
