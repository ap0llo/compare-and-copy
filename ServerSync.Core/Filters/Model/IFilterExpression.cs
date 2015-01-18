using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    public interface IFilterExpression
    {
        /// <summary>
        /// Determines if the specified <see cref="FileItem"/> has properties to fulfill the conditions defined by the filter element
        /// </summary>
        bool IsMatch(FileItem item);


        T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 paramter);

    }
}
