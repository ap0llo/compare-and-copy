using ServerSync.Core.State;
using ServerSync.Model;
using ServerSync.Model.Filtering;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
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

        #region Fields

        readonly string m_Name;
        readonly IImmutableList<IFilterExpression> m_IncludeRules;
        readonly IImmutableList<IFilterExpression> m_ExcludeRules;
        readonly ExpressionEvaluationVisitor m_Evaluator;

        #endregion

        #region Properties

        /// <summary>
        /// The filter's name
        /// </summary>
        public string Name { get { return m_Name; } }

        /// <summary>
        /// All filter elements that include an item in the result
        /// </summary>
        public IEnumerable<IFilterExpression> IncludeRules { get { return m_IncludeRules; } }

        /// <summary>
        /// The filter elements that exclude an item from the result
        /// </summary>
        public IEnumerable<IFilterExpression> ExcludeRules { get { return m_ExcludeRules; } }

        #endregion


        #region Constructor

        public LegacyFilter(string name, IEnumerable<IFilterExpression> includeRules, IEnumerable<IFilterExpression> excludeRules)
        {
            if(name == null)
            {
                throw new ArgumentNullException("name");
            }

            if(String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name must not be an empty string");
            }

            if(includeRules == null)
            {
                throw new ArgumentNullException("includeRules");
            }

            if(excludeRules == null)
            {
                throw new ArgumentNullException("excludeRules");
            }

            m_Name = name;
            m_IncludeRules = includeRules.ToImmutableList();
            m_ExcludeRules = excludeRules.ToImmutableList();


            var rootExpression = new AndFilterExpression(
                new OrFilterExpression(includeRules),
                new NotFilterExpression(new OrFilterExpression(excludeRules)));

            m_Evaluator = new ExpressionEvaluationVisitor(rootExpression);

        }

        #endregion

        #region Public Methods
        
        public IEnumerable<IFileItem> ApplyFilter(IEnumerable<IFileItem> filterInput)
        {
            return filterInput.Where(fileItem => m_Evaluator.IsMatch(fileItem));
        }
        
        #endregion


    }
}
