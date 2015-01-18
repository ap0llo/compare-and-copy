using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// Implementation of <see cref="IFilter"/>
    /// 
    /// Filter consists of a single filter expression that is evaluated in order to get the filter's result list    
    /// </summary>
    class Filter : IFilter
    {

        #region Fields
		
        readonly string m_Name;
        readonly IFilterExpression m_RootExpression;

	    #endregion


        #region Properties

        public string Name { get { return m_Name; } }  

        public IFilterExpression RootExpression { get { return m_RootExpression;}}        

        #endregion


        #region Constructor
        
        public Filter(string name, IFilterExpression rootExpression)
        {
            if(name == null)
            {
                throw new ArgumentNullException("name");
            }

            if(rootExpression == null)
            {
                throw new ArgumentNullException("rootExpression");
            }

            this.m_Name = name;
            this.m_RootExpression = rootExpression;
        }

        #endregion


        #region Public Methods
        
        public IEnumerable<FileItem> ApplyFilter(IEnumerable<FileItem> filterInput)
        {
            return filterInput.Where(file => this.RootExpression.IsMatch(file));
        }

        #endregion

    }
}
