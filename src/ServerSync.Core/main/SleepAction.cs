using ServerSync.Core.Configuration;
using ServerSync.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    class SleepAction : AbstractAction
    {


        #region Properties

        public override string Name
        {
            get { return "Sleep"; }
        }


        public TimeSpan Timeout { get; private set; }

        #endregion


        #region Constructor

        public SleepAction(bool isEnabled, ISyncConfiguration configuration, TimeSpan timeout)
            : base (isEnabled, configuration, null)
        {
            this.Timeout = timeout;
        }


        #endregion



        public override void Run()
        {
            Thread.Sleep(this.Timeout);
        }
    }
}
