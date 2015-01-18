using ServerSync.Core.State;
using System;
using System.Collections.Generic;
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

        #region Properties

        /// <summary>
        /// The filter's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All filter elements that include an item in the result
        /// </summary>
        public IEnumerable<IFilterExpression> IncludeRules { get; set; }

        /// <summary>
        /// The filter elements that exclude an item from the result
        /// </summary>
        public IEnumerable<IFilterExpression> ExcludeRules { get; set; }

        #endregion


        #region Constructor

        public LegacyFilter()
        {
            IncludeRules = Enumerable.Empty<IFilterExpression>();
            ExcludeRules = Enumerable.Empty<IFilterExpression>();
        }

        #endregion

        #region Public Methods
        
        public IEnumerable<FileItem> ApplyFilter(IEnumerable<FileItem> filterInput)
        {
            return filterInput.Where(item => IncludeRules.Any(rule => rule.IsMatch(item)))
                        .Where(item => !ExcludeRules.Any(rule => rule.IsMatch(item)));
        }
        
        #endregion


    }
}
