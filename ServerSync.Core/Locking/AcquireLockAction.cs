﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Locking
{
    class AcquireLockAction : LockAction
    {

        #region Fields

        TimeSpan? m_Timeout;

        #endregion
        

        #region Properties

        public override string Name
        {
            get { return "AcquireLock"; }
        }

        #endregion


        #region Constructor

        public AcquireLockAction(string lockFile) : this(lockFile, null)
        {
        }

        public AcquireLockAction(string lockFile, TimeSpan? timeout) : base(lockFile)
        {
            this.m_Timeout = timeout;
        }

        #endregion


        #region Public Methods

        public override void Run()
        {
            var success = false;
            var fileLock = FileLockFactory.GetFileLock(this.LockFile);

            if(this.m_Timeout.HasValue)
            {
                success = fileLock.Lock(this.m_Timeout.Value);
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

        #endregion

    }
}