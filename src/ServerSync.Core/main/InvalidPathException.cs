using System;

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
