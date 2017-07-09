using System;

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
