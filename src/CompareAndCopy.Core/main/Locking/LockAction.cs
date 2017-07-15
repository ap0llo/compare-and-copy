using CompareAndCopy.Model.Configuration;
using System;

namespace CompareAndCopy.Core.Locking
{
    abstract class LockAction : AbstractAction
    {
        public string LockFile { get; }


        protected LockAction(bool isEnabled, ISyncConfiguration configuration, string lockFile)
            : base(isEnabled, configuration, null)
        {
            if(String.IsNullOrEmpty(lockFile))
                throw new ArgumentNullException(lockFile);

            LockFile = lockFile;
        }
    }
}
