using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class CompareStateFilterElement : IFilterElement
    {

        private CompareState state;


        public CompareStateFilterElement(CompareState state)
        {
            this.state = state;
        }


        public bool IsMatch(FileItem item)
        {
            return item.CompareState == this.state;
        }
    }
}
