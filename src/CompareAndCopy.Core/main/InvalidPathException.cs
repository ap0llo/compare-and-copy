﻿using System;

namespace CompareAndCopy.Core
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
