using ServerSync.Model.Configuration;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Actions
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
