using ServerSync.Model.Configuration;
using System;

namespace ServerSync.Core.Locking
{
    class AcquireLockAction : LockAction
    {
        public override string Name => "AcquireLock";

        public TimeSpan? Timeout { get; }


        public AcquireLockAction(bool isEnabled, ISyncConfiguration configuration, string lockFile)
            : this(isEnabled, configuration, lockFile, null)
        {
        }

        public AcquireLockAction(bool isEnabled, ISyncConfiguration configuration, string lockFile, TimeSpan? timeout)
            : base(isEnabled, configuration, lockFile)
        {
            Timeout = timeout;
        }


        public override void Run()
        {
            var success = false;
            var fileLock = FileLockFactory.GetFileLock(this.LockFile);

            if(Timeout.HasValue)
            {
                success = fileLock.Lock(this.Timeout.Value);
            }
            else
            {
                success = fileLock.Lock();
            }

            if(!success)
            {
                throw new JobExecutionException("Failed to acquire file-lock within the specified timeout");
            }
        }
    }
}
