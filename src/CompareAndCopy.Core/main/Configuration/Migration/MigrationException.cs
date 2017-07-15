using System;

namespace CompareAndCopy.Core.Configuration.Migration
{
    [Serializable]
    class MigrationException : ConfigurationException
    {
        public MigrationException(string message) : base(message)
        {
        }
    }
}
