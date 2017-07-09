using System;
using System.Text.RegularExpressions;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter expression that filters based on the relative path of a <see cref="FileItem"/> and a regular expression
    /// To be evaluated to true if a FileItem's relative path matches the expression's regular expression
    /// </summary>
    public class RegexFilterExpression : IFilterExpression
    {
        /// <summary>
        /// The regular expression to be used to evaluate a FileItem's relative path
        /// </summary>
        public Regex Regex { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="RegexFilterExpression"/>
        /// </summary>
        /// <param name="pattern">The regular expression to be evaluated</param>
        public RegexFilterExpression(string pattern)
        {
            if(pattern == null)
                throw new ArgumentNullException(nameof(pattern));

            Regex = new Regex(pattern);
        }


        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter) => visitor.Visit(this, parameter);
    }
}
