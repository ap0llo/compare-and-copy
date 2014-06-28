using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    public class SyncState
    {

        private HashSet<FileItem> files;

        public IEnumerable<FileItem> Files
        {
            get
            {
                return files;
            }
            
        }



        public SyncState(IEnumerable<FileItem> files)
        {
            this.files = new HashSet<FileItem>(files);
        }


        public SyncState() : this(Enumerable.Empty<FileItem>())
        {

        }


        public void RemoveFile(FileItem item)
        {
            this.files.Remove(item);
        }
    }
}
