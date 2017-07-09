using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Base class for filter expressions encapsulating multiple other filter expressions
    /// </summary>
    public abstract class MultiFilterExpression : IFilterExpression
    {

        #region Fields

        readonly ImmutableList<IFilterExpression> m_Expressions;

        #endregion


        #region Properties

        /// <summary>
        /// The expression's child expressions
        /// </summary>
        public IImmutableList<IFilterExpression> Expressions { get { return m_Expressions; } }

        #endregion


        #region Constructor
        
        /// <summary>
        /// Constructs a new MultiFilterExpression
        /// </summary>
        /// <param name="expressions">The expressions encapsulated by the multi-expression</param>
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
