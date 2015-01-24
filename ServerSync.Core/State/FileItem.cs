using ServerSync.Model;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    public class FileItem : IFileItem
    {

        #region Fields

        readonly string m_RelativePath;

        #endregion


        #region IFileItem Implementation

        public string RelativePath { get { return m_RelativePath; } }

        public CompareState CompareState { get; set; }

        public TransferState TransferState { get; set; }

        #endregion


        #region Constructor
        
        public FileItem(string relativePath)
        {
            if(relativePath == null)
            {
                throw new ArgumentNullException("relativePath");
            }

            this.m_RelativePath = relativePath;
        }

        #endregion

    }
}
