using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    public class SyncState
    {

        #region Fields

        HashSet<FileItem> files;

        #endregion


        #region Properties

        public IEnumerable<FileItem> Files
        {
            get
            {
                return files;
            }
            
        }

        #endregion


        #region Constructor

        public SyncState(IEnumerable<FileItem> files)
        {
            this.files = new HashSet<FileItem>(files);
        }

        public SyncState() : this(Enumerable.Empty<FileItem>())
        {

        }

        #endregion


        #region Public Methods

        public void RemoveFile(FileItem item)
        {
            this.files.Remove(item);
        }
        
        #endregion

    }
}
