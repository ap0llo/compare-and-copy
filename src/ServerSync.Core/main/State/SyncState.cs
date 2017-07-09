using ServerSync.Model.State;
using System.Collections.Generic;
using System.Linq;

namespace ServerSync.Core.State
{
    public class SyncState : ISyncState
    {
        readonly HashSet<IFileItem> m_Files;


        public IEnumerable<IFileItem> Files => m_Files;


        public SyncState() : this(Enumerable.Empty<IFileItem>())
        {
        }

        public SyncState(IEnumerable<IFileItem> files)
        {
            m_Files = new HashSet<IFileItem>(files);
        }


        public void RemoveFile(IFileItem item) => m_Files.Remove(item);
    }
}
