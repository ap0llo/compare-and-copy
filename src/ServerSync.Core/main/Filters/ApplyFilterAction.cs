using ServerSync.Core.State;
using ServerSync.Model.Configuration;

namespace ServerSync.Core.Filters
{
    class ApplyFilterAction : AbstractAction
    {
        public override string Name => "ApplyFilter";
        

        public ApplyFilterAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName)
            : base(isEnabled, configuration, inputFilterName)
        {
        }

        
        public override void Run()
        { 
            State = new SyncState(GetFilteredInput());            
        }        
    }
}
