using System;

namespace ServerSync.Core.Configuration
{
    public class TransferLocationReference
    {
        public string TransferLocationName { get; }

        public string TransferLocationSubPath { get; }


        public TransferLocationReference(string transferLocationName, string transferLocationSubPath)
        {
            TransferLocationName = transferLocationName ?? throw new ArgumentNullException(nameof(transferLocationName));
            TransferLocationSubPath = transferLocationSubPath ?? throw new ArgumentNullException(nameof(transferLocationSubPath));
        }
    }
}
