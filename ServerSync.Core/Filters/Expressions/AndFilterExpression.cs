using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    public class AndFilterExpression : MultiFilterExpression
    {

        #region Constructor
        
        public AndFilterExpression(IEnumerable<IFilterExpression> expressions) : base(expressions)
        {
        }

        #endregion


        #region Overrides

        public override bool IsMatch(FileItem item)
        {
            return this.Expressions.All(expression => expression.IsMatch(item));
        }

        public override T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}
