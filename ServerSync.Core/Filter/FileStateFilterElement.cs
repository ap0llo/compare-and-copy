using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class FileStateFilterElement : IFilterElement
    {

        private FileState state;


        public FileStateFilterElement(FileState state)
        {
            this.state = state;
        }


        public bool IsMatch(FileItem item)
        {
            return item.State == this.state;
        }
    }
}
