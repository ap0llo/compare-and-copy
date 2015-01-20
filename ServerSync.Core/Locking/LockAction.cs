using ServerSync.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Locking
{
    abstract class LockAction : AbstractAction
    {
        #region Fields

        readonly string m_LockFile;

        #endregion

        #region Properties

        public string LockFile { get { return m_LockFile; } }

        #endregion


        #region Constructor

        public LockAction(bool isEnabled, ISyncConfiguration configuration, string lockFile)
            : base(isEnabled, configuration, null)
        {
            if(String.IsNullOrEmpty(lockFile))
            {
                throw new ArgumentNullException(lockFile);
            }
            this.m_LockFile = lockFile;
        }

        #endregion

    }
}
