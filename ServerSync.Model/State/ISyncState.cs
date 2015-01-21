using ServerSync.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSync.Core
{
    public interface ISyncState
    {


        IEnumerable<IFileItem> Files { get; }
        

        void RemoveFile(IFileItem item);
        

    }
}
