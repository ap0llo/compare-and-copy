using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace CompareAndCopy.Model.Filtering
{
    /// <summary>
    /// Base class for filter expressions encapsulating multiple other filter expressions
    /// </summary>
    public abstract class MultiFilterExpression : IFilterExpression
    {
        readonly ImmutableList<IFilterExpression> m_Expressions;

        /// <summary>
        /// The expression's child expressions
        /// </summary>
        public IImmutableList<IFilterExpression> Expressions => m_Expressions;


        /// <summary>
        /// Constructs a new MultiFilterExpression
        /// </summary>
        /// <param name="expressions">The expressions encapsulated by the multi-expression</param>
        protected MultiFilterExpression(IEnumerable<IFilterExpression> expressions)
        {
            m_Expressions = expressions?.ToImmutableList() ?? throw new ArgumentNullException(nameof(expressions));
        }

        
        public abstract T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter);
    }
}
