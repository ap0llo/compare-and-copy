using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration
{
    public class TransferLocationReference
    {

        public string TransferLocationName { get; private set; }

        public string TransferLocationSubPath { get; private set; }


        public TransferLocationReference(string transferLocationName, string transferLocationSubPath)
        {
            if(transferLocationName == null)
            {
                throw new ArgumentNullException("transferLocationName");
            }

            if(transferLocationSubPath == null)
            {
                throw new ArgumentNullException("transferLocationSubPath");
            }

            this.TransferLocationName = transferLocationName;
            this.TransferLocationSubPath = transferLocationSubPath;
        }

    }
}
