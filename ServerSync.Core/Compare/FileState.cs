﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Compare
{
    enum FileState
    {
        MissingLeft,
        MissingRight,
        Conflict,
        InTransferToLeft,
        InTransferToRight
    }
}
