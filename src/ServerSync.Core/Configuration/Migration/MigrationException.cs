using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration.Migration
{
    [Serializable]
    class MigrationException : ConfigurationException
    {

        public MigrationException(string message) : base(message)
        {

        }

    }
}
