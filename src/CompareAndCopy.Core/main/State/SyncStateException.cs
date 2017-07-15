using System;

namespace CompareAndCopy.Core.State
{
    [Serializable]
    class SyncStateException : Exception
    {
        public SyncStateException(string message) : base (message)
        {
        }
    }
}
