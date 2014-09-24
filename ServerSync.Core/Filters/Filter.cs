using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// Filter that can be applied to a <see cref="SyncState"/>
    /// A filter consits of two collections of <see cref="IFilterElement"/> which define what FileItems are included/excluded by the filter
    /// </summary>
    public class Filter
    {

        #region Properties

        /// <summary>
        /// The filter's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// All filter elements that include an item in the result
        /// </summary>
        public IEnumerable<IFilterElement> IncludeRules { get; set; }

        /// <summary>
        /// The filter elements that exclude an item from the result
        /// </summary>
        public IEnumerable<IFilterElement> ExcludeRules { get; set; }

        #endregion


        #region Constructor

        public Filter()
        {
            IncludeRules = Enumerable.Empty<IFilterElement>();
            ExcludeRules = Enumerable.Empty<IFilterElement>();
        }

        #endregion

    }
}
