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

        bool IsEnabled { get; }

        string InputFilterName { get; }

        ISyncConfiguration Configuration { get; }

        ISyncState State { get; set; }

        void Run();
    }
}
