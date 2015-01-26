using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
