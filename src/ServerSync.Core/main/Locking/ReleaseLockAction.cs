﻿using ServerSync.Model.Configuration;

namespace ServerSync.Core.Locking
{
    class ReleaseLockAction : LockAction
    {
        public override string Name => "ReleaseLock";


        public ReleaseLockAction(bool isEnabled, ISyncConfiguration configuration, string lockFile)
            : base(isEnabled, configuration, lockFile)
        {
        }
        

        public override void Run() => FileLockFactory.GetFileLock(LockFile).Unlock();
    }
}
