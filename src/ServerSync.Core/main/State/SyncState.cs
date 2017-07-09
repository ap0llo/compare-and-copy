using ServerSync.Model;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    public class SyncState : ISyncState
    {

        #region Fields

        HashSet<IFileItem> files;

        #endregion


        #region Properties

        public IEnumerable<IFileItem> Files
        {
            get
            {
                return files;
            }
            
        }

        #endregion


        #region Constructor

        public SyncState(IEnumerable<IFileItem> files)
        {
            this.files = new HashSet<IFileItem>(files);
        }

        public SyncState() : this(Enumerable.Empty<IFileItem>())
        {

        }

        #endregion


        #region Public Methods

        public void RemoveFile(IFileItem item)
        {
            this.files.Remove(item);
        }
        
        #endregion

    }
}
