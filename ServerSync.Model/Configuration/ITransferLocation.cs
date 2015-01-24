using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Configuration
{
    public interface ITransferLocation
    {
        string Name { get; }

        string Path { get; }

        ByteSize.ByteSize? MaximumSize { get; }

    }
}
