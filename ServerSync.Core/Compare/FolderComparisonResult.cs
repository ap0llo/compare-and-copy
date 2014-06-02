using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Compare
{
    public class FolderComparisonResult
    {
        private IEnumerable<string> missingLeft;
        private IEnumerable<string> missingRight;
        private IEnumerable<string> conflicts;
        private IEnumerable<string> sameFiles;


        public FolderComparisonResult(IEnumerable<string> missingLeft, IEnumerable<string> missingRight,
            IEnumerable<string> conflicts, IEnumerable<string> sameFiles)
        {
            this.missingLeft = missingLeft;
            this.missingRight = missingRight;
            this.conflicts = conflicts;
            this.sameFiles = sameFiles;
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


    }
}
