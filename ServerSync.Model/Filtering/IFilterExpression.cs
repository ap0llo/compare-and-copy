using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    public interface IFilterExpression
    { 

        T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 paramter);

    }
}
