using ServerSync.Core.Copy;
using ServerSync.Core.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration
{
    public class SyncConfiguration
    {

        #region Fields

        private Dictionary<string, IFilter> filters = new Dictionary<string, IFilter>();
        private Dictionary<string, TransferLocation> transferLocations = new Dictionary<string,TransferLocation>();
        private List<IAction> actions = new List<IAction>();

        #endregion Fields


        #region Properties

        public SyncFolderDefinition Left { get; set; }

        public SyncFolderDefinition Right { get; set; }

        public TimeSpan TimeStampMargin { get; set; }
        
        public IEnumerable<IFilter> Filters
        {
            get { return this.filters.Values; }
        }        

        public IEnumerable<IAction> Actions 
        {
            get { return this.actions; } 
        }

        public IEnumerable<TransferLocation> TransferLocations
        {
            get { return transferLocations.Values; }
        }

        #endregion Properties


        #region Public Methods

        public void AddFilter(IFilter filter)
        {
            string key = GetFilterKey(filter.Name);
            if(this.filters.ContainsKey(key))
            {
                this.filters[key] = filter;
            }
            else
            {
                this.filters.Add(key, filter);
            }
        }

        public IFilter GetFilter(string name)
        {
            return this.filters[GetFilterKey(name)];
        }

        public void AddAction(IAction action)
        {
            this.actions.Add(action);
        }

        public void AddTransferLocation(TransferLocation transferLocation)
        {
            this.transferLocations.Add(GetTransferLocationKey(transferLocation.Name), transferLocation);
        }

        public TransferLocation GetTransferLocation(string name)
        {
            return this.transferLocations[GetTransferLocationKey(name)];
        }

        #endregion Public Methods


        #region Private Implementation

        string GetFilterKey(string name)
        {
            return name.ToLower().Trim();
        }

        string GetTransferLocationKey(string name)
        {
            return name.ToLower().Trim();
        }

        #endregion Private Implementation

    }
}
