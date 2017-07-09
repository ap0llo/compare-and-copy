using System;

namespace ServerSync.Core
{
    [Serializable]
    class JobExecutionException : Exception 
    {
        public JobExecutionException(string message) : base(message)
        {
        }
    }
}
