using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration
{
    [Serializable]
    public class ConfigurationException : Exception
    {

        public ConfigurationException(string message) : base(message)
        {

        }

        public ConfigurationException(string message, Exception innerException) : base(message, innerException)
        {

        }

    }
}
