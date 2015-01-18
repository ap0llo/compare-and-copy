using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    public interface IFilter
    {
        string Name { get; }

        IEnumerable<FileItem> ApplyFilter(IEnumerable<FileItem> filterInput);

    }
}
