﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    class SyncStateException : Exception
    {
        public SyncStateException(string message) : base (message)
        {

        }

    }
}