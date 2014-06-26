using ServerSync.Core.Compare;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    class CopyAction : IAction
    {

        private List<string> missingLeft;
        private List<string> missingRight;
        private List<string> conflict;
        private List<string> inTransferToRight;
        private List<string> inTransferToLeft;




        public bool IsEnabled { get; set; }

        public SyncConfiguration Configuration { get; set; }

        public SyncState State { get; set; }


        public FileState ItemType { get; set; }
        public FileState SetStateTo { get; set; }

        public string TargetDirectory { get; set; }

        public Source Source { get; set; }


        public void Run()
        {
            this.missingLeft = State.MissingLeft.ToList();
            this.missingRight = State.MissingRight.ToList();
            this.conflict = State.Conflicts.ToList();
            this.inTransferToLeft = State.InTransferToLeft.ToList();
            this.inTransferToRight = State.InTransferToRight.ToList();

            var root = Source == Source.Left ? Configuration.Left.RootPath : Configuration.Right.RootPath;

            var copyItems = GetItemsToCopy();
            foreach (var item in copyItems)
            {
                var absSource = Path.Combine(root, item);
                var absTarget = Path.Combine(TargetDirectory, item);

                Console.WriteLine("Copying {0}", item);

                EnsureDirectoryExists(Path.GetDirectoryName(absTarget));
                File.Copy(absSource, absTarget);
                SetNewState(item);
            }
            

            this.State = new SyncState(missingLeft, missingRight, conflict, Enumerable.Empty<string>(), inTransferToLeft, inTransferToRight);

        }



        private IEnumerable<string> GetItemsToCopy()
        {
            switch (ItemType)
            {
                case FileState.MissingLeft:
                    this.missingLeft = new List<string>();
                    return this.State.MissingLeft;

                case FileState.MissingRight:
                    this.missingRight = new List<string>();
                    return this.State.MissingRight;
                    
                case FileState.Conflict:
                    this.conflict = new List<string>();
                    return this.State.Conflicts;

                case FileState.InTransferToLeft:
                    this.inTransferToLeft = new List<string>();
                    return this.State.InTransferToLeft;
                    
                case FileState.InTransferToRight:
                    this.inTransferToRight = new List<string>();
                    return this.State.InTransferToRight;

                default:
                    throw new NotImplementedException();                    
            }
        }

        private void SetNewState(string item)
        {
            switch (this.SetStateTo)
            {
                case FileState.MissingLeft:
                    missingLeft.Add(item);
                    break;

                case FileState.MissingRight:
                    missingRight.Add(item);
                    break;

                case FileState.Conflict:
                    conflict.Add(item);
                    break;

                case FileState.InTransferToLeft:
                    inTransferToLeft.Add(item);
                    break;

                case FileState.InTransferToRight:
                    inTransferToRight.Add(item);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }


        private void EnsureDirectoryExists(string path)
        {
            if(!Directory.Exists(path))
            {
                var parent = Path.GetDirectoryName(path);
                EnsureDirectoryExists(parent);

                Directory.CreateDirectory(path);
            }
        }

    }
}
