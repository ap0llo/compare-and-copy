using System;

namespace CompareAndCopy.Core
{
    [Serializable]
    class JobExecutionException : Exception 
    {
        public JobExecutionException(string message) : base(message)
        {
        }
    }
}
