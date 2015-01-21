using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace ServerSync.Core.Filters
{
    public abstract class MultiFilterExpression : IFilterExpression
    {

        #region Fields

        readonly ImmutableList<IFilterExpression> m_Expressions;

        #endregion


        #region Properties

        public IImmutableList<IFilterExpression> Expressions { get { return m_Expressions; } }

        #endregion


        #region Constructor
        
        public MultiFilterExpression(IEnumerable<IFilterExpression> expressions)
        {
            if(expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }

            this.m_Expressions = expressions.ToImmutableList();
        }

        #endregion


        #region IFilterExpression Implementation
        
        public abstract T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter);
       
        #endregion

    }
}
