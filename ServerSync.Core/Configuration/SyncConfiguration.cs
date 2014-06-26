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

        private Dictionary<string, Filter> filters = new Dictionary<string,Filter>();
        private List<IAction> actions = new List<IAction>();

        #endregion Fields


        #region Properties

        public SyncFolder Left { get; set; }

        public SyncFolder Right { get; set; }

        public long TimeStampMargin { get; set; }
        
        public IEnumerable<Filter> Filters
        {
            get
            {
                return this.filters.Values;
            }
        }        

        public IEnumerable<IAction> Actions { get { return this.actions; } }

        #endregion Properties


        #region Constructor

        public SyncConfiguration()
        {
            
        }


        #endregion


        #region Public Methods

        public void AddFilter(Filter filter)
        {
            string key = filter.Name.ToLower().Trim();
            if(this.filters.ContainsKey(key))
            {
                this.filters[key] = filter;
            }
            else
            {
                this.filters.Add(key, filter);
            }
        }

        public Filter GetFilter(string name)
        {
            return this.filters[name];
        }

        public void AddAction(IAction action)
        {
            this.actions.Add(action);
        }

        #endregion Public Methods

    }
}
