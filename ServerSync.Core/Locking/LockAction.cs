using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Locking
{
    abstract class LockAction : AbstractAction
    {

        #region Properties

        public string LockFile { get; private set; }

        #endregion


        #region Constructor

        public LockAction(string lockFile)
        {
            if(String.IsNullOrEmpty(lockFile))
            {
                throw new ArgumentNullException(lockFile);
            }
            this.LockFile = lockFile;
        }

        #endregion

    }
}
