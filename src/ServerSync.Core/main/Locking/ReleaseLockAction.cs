using ServerSync.Core.Configuration;
using ServerSync.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Locking
{
    class ReleaseLockAction : LockAction
    {
        
        #region Properties

        public override string Name
        {
            get { return "ReleaseLock"; }
        }

        #endregion


        #region Constructor

        public ReleaseLockAction(bool isEnabled, ISyncConfiguration configuration, string lockFile)
            : base(isEnabled, configuration, lockFile)
        {

        }

        #endregion


        #region Public Methods

        public override void Run()
        {
            var fileLock = FileLockFactory.GetFileLock(this.LockFile);
            fileLock.Unlock();
        }

        #endregion

    }
}
