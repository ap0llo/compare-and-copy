using ServerSync.Core.State;
using ServerSync.Model;
using ServerSync.Model.Filtering;
using ServerSync.Model.State;
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
        readonly ExpressionEvaluationVisitor m_Evaluator;

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
            this.m_Evaluator = new ExpressionEvaluationVisitor(this.RootExpression);
        }

        #endregion


        #region Public Methods
        
        public IEnumerable<IFileItem> ApplyFilter(IEnumerable<IFileItem> filterInput)
        {
            return filterInput.Where(file => this.m_Evaluator.IsMatch(file));
        }

        #endregion

    }
}
