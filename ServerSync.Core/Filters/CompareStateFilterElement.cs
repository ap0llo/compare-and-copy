using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// Filter element that filters based on the "CompareState" property of a <see cref="FileItem"/>
    /// </summary>
    class CompareStateFilterElement : IFilterElement
    {

        #region Fields

        private CompareState state;

        #endregion


        #region Constructor

        public CompareStateFilterElement(CompareState state)
        {
            this.state = state;
        }

        #endregion


        #region IFilterElement Implementation

        public bool IsMatch(FileItem item)
        {
            return item.CompareState == this.state;
        }

        #endregion

    }
}
