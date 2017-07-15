using System;

namespace CompareAndCopy.Model.Filtering
{
    /// <summary>
    /// Defines a filter expression that uses the Microscope framework (https://github.com/clotheshorse/microscope) to find matches.
    /// The Microscope query is applied to RelativePath property of <see cref="FileItem"/>
    /// </summary>
    public class MicroscopeFilterExpression : IFilterExpression
    {
        /// <summary>
        /// The Microscope query to be evaluated
        /// </summary>
        public string Query { get; }

        
        public MicroscopeFilterExpression(string query)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));            
        }


        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }
    }
}
