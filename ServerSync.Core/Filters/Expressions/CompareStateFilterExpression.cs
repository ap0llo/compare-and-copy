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
    public class CompareStateFilterExpression : IFilterExpression
    {

        #region Fields

        private CompareState state;

        #endregion


        #region Properties

        public CompareState CompareState { get { return this.state; } }

        #endregion

        #region Constructor

        public CompareStateFilterExpression(CompareState state)
        {
            this.state = state;
        }

        #endregion


        #region IFilterExpression Implementation

        public bool IsMatch(FileItem item)
        {
            return item.CompareState == this.state;
        }

        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}
