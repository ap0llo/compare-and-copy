using CompareAndCopy.Model.Configuration;
using System;
using System.Threading;

namespace CompareAndCopy.Core
{
    class SleepAction : AbstractAction
    {
        public override string Name => "Sleep";

        public TimeSpan Timeout { get; }
       

        public SleepAction(bool isEnabled, ISyncConfiguration configuration, TimeSpan timeout)
            : base (isEnabled, configuration, null)
        {
            Timeout = timeout;
        }
        

        public override void Run() => Thread.Sleep(this.Timeout);
    }
}
