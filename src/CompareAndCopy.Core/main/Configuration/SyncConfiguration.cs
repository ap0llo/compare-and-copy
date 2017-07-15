using CompareAndCopy.Model.Actions;
using CompareAndCopy.Model.Configuration;
using CompareAndCopy.Model.Filtering;
using System.Collections.Generic;

namespace CompareAndCopy.Core.Configuration
{
    public class SyncConfiguration : ISyncConfiguration
    {
        readonly Dictionary<string, IFilter> m_Filters = new Dictionary<string, IFilter>();
        readonly Dictionary<string, ITransferLocation> m_TransferLocations = new Dictionary<string, ITransferLocation>();
        readonly List<IAction> m_Actions = new List<IAction>();


        public ISyncFolderDefinition Left { get; set; }

        public ISyncFolderDefinition Right { get; set; }   
        
        public IEnumerable<IFilter> Filters => m_Filters.Values;

        public IEnumerable<IAction> Actions => m_Actions;

        public IEnumerable<ITransferLocation> TransferLocations => m_TransferLocations.Values;


        public void AddFilter(IFilter filter)
        {
            var key = GetFilterKey(filter.Name);
            if(m_Filters.ContainsKey(key))
            {
                m_Filters[key] = filter;
            }
            else
            {
                m_Filters.Add(key, filter);
            }
        }

        public IFilter GetFilter(string name) => m_Filters[GetFilterKey(name)];

        public void AddAction(IAction action) => m_Actions.Add(action);

        public void AddTransferLocation(ITransferLocation transferLocation) 
            => m_TransferLocations.Add(GetTransferLocationKey(transferLocation.Name), transferLocation);

        public ITransferLocation GetTransferLocation(string name) 
            => m_TransferLocations[GetTransferLocationKey(name)];


        string GetFilterKey(string name) => name.ToLower().Trim();

        string GetTransferLocationKey(string name) => name.ToLower().Trim();
    }
}
