using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    public interface IFilterElement
    {
        /// <summary>
        /// Determines if the specified <see cref="FileItem"/> has properties to fulfill the conditions defined by the filter element
        /// </summary>
        bool IsMatch(FileItem item);
    }
}
