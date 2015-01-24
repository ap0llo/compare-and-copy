
using ServerSync.Model.Actions;
using ServerSync.Model.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Configuration
{
    public interface ISyncConfiguration
    {

        #region Properties

        ISyncFolderDefinition Left { get; set; }

        ISyncFolderDefinition Right { get; set; }

        TimeSpan TimeStampMargin { get; set; }

        IEnumerable<IFilter> Filters { get;}
        
        IEnumerable<IAction> Actions { get;}

        IEnumerable<ITransferLocation> TransferLocations { get; }

        #endregion 


        #region Public Methods

        void AddFilter(IFilter filter);
        
        IFilter GetFilter(string name);
        
        void AddAction(IAction action);

        void AddTransferLocation(ITransferLocation transferLocation);
        
        ITransferLocation GetTransferLocation(string name);       

        #endregion

    }
}
