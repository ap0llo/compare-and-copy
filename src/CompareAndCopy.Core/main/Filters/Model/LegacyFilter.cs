using CompareAndCopy.Model.Filtering;
using CompareAndCopy.Model.State;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using CompareAndCopy.Core.State;

namespace CompareAndCopy.Core.Filters
{
    /// <summary>
    /// Legacy implementation of filter
    /// 
    /// Filter that can be applied to a <see cref="SyncState"/>
    /// A filter consists of two collections of <see cref="IFilterExpression"/> which define what FileItems are included/excluded by the filter
    /// </summary>
    [Obsolete]
    class LegacyFilter : IFilter
    {
        readonly IImmutableList<IFilterExpression> m_IncludeRules;
        readonly IImmutableList<IFilterExpression> m_ExcludeRules;
        readonly ExpressionEvaluationVisitor m_Evaluator;


        /// <summary>
        /// The filter's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// All filter elements that include an item in the result
        /// </summary>
        public IEnumerable<IFilterExpression> IncludeRules => m_IncludeRules;

        /// <summary>
        /// The filter elements that exclude an item from the result
        /// </summary>
        public IEnumerable<IFilterExpression> ExcludeRules => m_ExcludeRules;


        public LegacyFilter(string name, IEnumerable<IFilterExpression> includeRules, IEnumerable<IFilterExpression> excludeRules)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value must not be null or whitespace", nameof(name));

            Name = name;
            m_IncludeRules = includeRules?.ToImmutableList() ?? throw new ArgumentNullException(nameof(includeRules));
            m_ExcludeRules = excludeRules?.ToImmutableList() ?? throw new ArgumentNullException(nameof(excludeRules));

            var rootExpression = new AndFilterExpression(
                new OrFilterExpression(includeRules),
                new NotFilterExpression(new OrFilterExpression(excludeRules)));

            m_Evaluator = new ExpressionEvaluationVisitor(rootExpression);

        }

        
        public IEnumerable<IFileItem> ApplyFilter(IEnumerable<IFileItem> filterInput)
            => filterInput.Where(fileItem => m_Evaluator.IsMatch(fileItem));
    }
}
