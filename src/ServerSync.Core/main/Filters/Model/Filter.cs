using ServerSync.Model.Filtering;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// Implementation of <see cref="IFilter"/>
    /// 
    /// Filter consists of a single filter expression that is evaluated in order to get the filter's result list    
    /// </summary>
    class Filter : IFilter
    {
        readonly ExpressionEvaluationVisitor m_Evaluator;

        
        public string Name { get; }

        public IFilterExpression RootExpression { get; }


        public Filter(string name, IFilterExpression rootExpression)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RootExpression = rootExpression ?? throw new ArgumentNullException(nameof(rootExpression));
            m_Evaluator = new ExpressionEvaluationVisitor(this.RootExpression);
        }

        
        public IEnumerable<IFileItem> ApplyFilter(IEnumerable<IFileItem> filterInput) 
            => filterInput.Where(file => this.m_Evaluator.IsMatch(file));
    }
}
