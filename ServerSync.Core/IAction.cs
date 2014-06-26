using ServerSync.Core.Compare;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    public interface IAction
    {
        string Name { get; }

        bool IsEnabled { get; set; }

        SyncConfiguration Configuration { get; set; }

        SyncState State { get; set; }

        void Run();
    }
}
