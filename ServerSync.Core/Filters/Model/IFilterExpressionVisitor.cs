using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
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
