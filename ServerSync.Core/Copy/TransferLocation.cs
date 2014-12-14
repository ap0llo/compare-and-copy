using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    public class TransferLocation
    {

        #region Properties

        public string Name { get; private set; }

        public string Path { get; private set; }

        public ByteSize.ByteSize? MaximumSize { get; private set; }

        #endregion


        #region Constructor

        public TransferLocation(string name, string path, ByteSize.ByteSize? maximumSize)
        {
            this.Name = name;
            this.Path = path;
            this.MaximumSize = maximumSize;
        }

        #endregion

    }
}
