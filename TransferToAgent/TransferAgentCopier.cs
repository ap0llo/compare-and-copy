using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ServerSync.Core;
using Renci.SshNet;

namespace TransferToAgent
{
    class TransferAgentCopier
    {


        #region Properties

        public string TransferAgentRoot { get; set; }

        public string TransferLocationRoot { get; set; }


        public IEnumerable<string> IncomingFolders { get; set; }

        public IEnumerable<string> OutgoingFolders { get; set; }


        public string TransferAgentName { get; set; }

        public string TransferAgentUserName { get; set; }

        public string TransferAgentPassword { get; set; }

        #endregion 


        #region Public Methods
        
        public void Run()
        {
            MoveIncomingFiles();

        }

        #endregion


        #region Private Methods

        private void MoveIncomingFiles()
        {
            foreach (var dirName in IncomingFolders)
            {

                var sourceRoot = Path.Combine(TransferAgentRoot, dirName);
                var targetRoot = Path.Combine(TransferLocationRoot, dirName);


                var files = GetFiles(sourceRoot);

                foreach(var file in files)
                {
                    if(!file.EndsWith("!sync", StringComparison.InvariantCultureIgnoreCase) && !file.StartsWith(".sync", StringComparison.InvariantCulture))
                    {

                        var target = Path.Combine(targetRoot, IOHelper.GetRelativePath(file, sourceRoot, true));

                        File.Copy(file, target, true);
                        File.Delete(file);


                    }
                }
            }
        }



        private void MoveOutgoingFiles()
        {
            foreach (var dirName in OutgoingFolders)
            {
                var transferShareRoot = Path.Combine(TransferLocationRoot, dirName);
                var transferAgentRoot = Path.Combine(TransferAgentRoot, dirName);

            }

        }

        private IEnumerable<string> GetFiles(string directory)
        {
            return Directory.GetFiles(directory).Union(Directory.GetDirectories(directory)
                    .Where(dirPath => !Path.GetFileName(dirPath).Equals(".sync"))
                    .SelectMany(GetFiles));
        }


        private long GetSizeOnTransferAgent()
        {
            var sshClient = new SshClient(TransferAgentName, TransferAgentUserName, TransferAgentPassword);
            sshClient.Connect();


            var command = sshClient.RunCommand("echo foo");
            

            throw new NotImplementedException();
        }


        #endregion


    }
}
