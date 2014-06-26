using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    public class SyncState
    {
        private IEnumerable<string> missingLeft;
        private IEnumerable<string> missingRight;
        private IEnumerable<string> conflicts;
        private IEnumerable<string> sameFiles;
        private IEnumerable<string> inTransferToLeft;
        private IEnumerable<string> inTransferToRight;

        public SyncState() : this(Enumerable.Empty<string>(), Enumerable.Empty<string>(), Enumerable.Empty<string>(), Enumerable.Empty<string>())
        {

        }

        public SyncState(IEnumerable<string> missingLeft, IEnumerable<string> missingRight,
            IEnumerable<string> conflicts, IEnumerable<string> sameFiles)
            : this(missingLeft, missingRight, conflicts, sameFiles, Enumerable.Empty<string>(), Enumerable.Empty<string>())
        {
        }

        public SyncState( IEnumerable<string> missingLeft, IEnumerable<string> missingRight,
                          IEnumerable<string> conflicts, IEnumerable<string> sameFiles, 
                          IEnumerable<string> inTransferToLeft, IEnumerable<string> inTransferToRight )
        {
            this.missingLeft = missingLeft;
            this.missingRight = missingRight;
            this.conflicts = conflicts;
            this.sameFiles = sameFiles;
            this.inTransferToRight = inTransferToRight;
            this.inTransferToLeft = inTransferToLeft;
        }


        public IEnumerable<string> MissingLeft
        {
            get
            {
                return this.missingLeft;
            }
        }

        public IEnumerable<string> MissingRight
        {
            get
            {
                return this.missingRight;
            }
        }

        public IEnumerable<string> Conflicts
        {
            get
            {
                return this.conflicts;
            }
        }

        public IEnumerable<string> SameFiles
        {
            get
            {
                return this.sameFiles;
            }
        }

        public IEnumerable<string> InTransferToLeft
        {
            get
            {
                return this.inTransferToLeft;
            }
            set
            {
                this.inTransferToLeft = value;
            }
        }

        public IEnumerable<string> InTransferToRight
        {
            get
            {
                return this.inTransferToRight;
            }
            set
            {
                this.inTransferToRight = value;
            }
        }

    }
}
