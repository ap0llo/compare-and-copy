using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSync.Model;

namespace ServerSync.Core.Filters
{
    public interface IFilter
    {
        string Name { get; }

        IEnumerable<IFileItem> ApplyFilter(IEnumerable<IFileItem> filterInput);

    }
}
