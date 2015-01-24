using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    public class AndFilterExpression : MultiFilterExpression
    {

        #region Constructor
        
        public AndFilterExpression(params  IFilterExpression[] expressions) : this((IEnumerable<IFilterExpression>)expressions)
        {
        }

        public AndFilterExpression(IEnumerable<IFilterExpression> expressions) : base(expressions)
        {
        }

        #endregion


        #region Overrides

        public override T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}
