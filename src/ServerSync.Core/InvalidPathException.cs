﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    [Serializable]
    public class InvalidPathException : Exception
    {


        public InvalidPathException(string message)
            : base(message)
        {

        }

    }
}