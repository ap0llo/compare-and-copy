using CompareAndCopy.Model.Configuration;
using System;

namespace CompareAndCopy.Core.Configuration
{
    public class SyncFolderDefinition : ISyncFolderDefinition
    {
        public string Name { get; }

        public string RootPath { get; }

        public SyncFolder Role { get; }

        
        public SyncFolderDefinition(string name, string rootPath, SyncFolder role)
        {
            if(name == null)
                throw new ArgumentNullException(nameof(name));

            if(rootPath == null)
                throw new ArgumentNullException(nameof(rootPath));

            if(String.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value must not be empty", nameof(name));

            Name = name;
            RootPath = rootPath;
            Role = role;
        }
    }
}
