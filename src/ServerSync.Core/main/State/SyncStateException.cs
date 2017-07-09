using System;
namespace ServerSync.Core.State
{
    [Serializable]
    class SyncStateException : Exception
    {
        public SyncStateException(string message) : base (message)
        {
        }
    }
}
