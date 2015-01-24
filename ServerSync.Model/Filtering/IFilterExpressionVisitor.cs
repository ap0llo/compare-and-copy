
namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Interface for visitors to traverse the expression tree of a filter
    /// </summary>
    /// <typeparam name="T1">The visitor's return type</typeparam>
    /// <typeparam name="T2">The type of the parameter passed to the visitor's Visit() methods</typeparam>
    public interface IFilterExpressionVisitor<T1, T2>
    {
        T1 Visit(AndFilterExpression expression, T2 paramter);

        T1 Visit(OrFilterExpression expression, T2 paramter);
        
        T1 Visit(NotFilterExpression expression, T2 paramter);
        
        T1 Visit(RegexFilterExpression expression, T2 paramter);
        
        T1 Visit(MicroscopeFilterExpression expression, T2 paramter);
        
        T1 Visit(CompareStateFilterExpression expression, T2 paramter);
        
        T1 Visit(TransferStateFilterExpression expression, T2 paramter);
    }
}
