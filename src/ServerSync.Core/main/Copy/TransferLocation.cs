using ServerSync.Model.Configuration;
using System;
using ByteSizeLib;

namespace ServerSync.Core.Copy
{
    public class TransferLocation : ITransferLocation
    {
        public string Name { get; }

        public string RootPath { get; }

        public ByteSize? MaximumSize { get; }


        public TransferLocation(string name, string path, ByteSize? maximumSize)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name)); ;

            if(path == null)
                throw new ArgumentNullException(nameof(path));

            if(String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value must not be empty", nameof(name));

            Name = name;
            RootPath = path;
            MaximumSize = maximumSize;
        }
    }
}
