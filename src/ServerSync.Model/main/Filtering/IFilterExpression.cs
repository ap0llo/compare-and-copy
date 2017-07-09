namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Interface for nodes in the expression tree defined by filters
    /// </summary>
    public interface IFilterExpression
    { 
        /// <summary>
        /// Calls the appropriate Visit() Method on the specified visitor passing in the specified parameter 
        /// </summary>
        /// <typeparam name="T1">The type of the value returned by the visitor</typeparam>
        /// <typeparam name="T2">The type of the parameter to pass to the visitor</typeparam>
        /// <param name="visitor">The visitor to call</param>
        /// <param name="paramter">The parameter to pass to the visitor</param>
        /// <returns>Returns the value returned by the visitor's Visit() method</returns>
        T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 paramter);
    }
}
