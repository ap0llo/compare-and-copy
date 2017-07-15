using CompareAndCopy.Core.State;
using CompareAndCopy.Model.Configuration;

namespace CompareAndCopy.Core.Filters
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
