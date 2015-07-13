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
        readonly string m_NormalizedRelativePath;
        readonly TransferState m_TransferState;

        #endregion


        #region IFileItem Implementation

        public string RelativePath { get { return m_RelativePath; } }

        public CompareState CompareState { get; set; }

        public TransferState TransferState { get { return m_TransferState; } }

        #endregion


        #region Constructor

        public FileItem(string relativePath) : this(relativePath, new TransferState())
        {

        }

        public FileItem(string relativePath, TransferState transferState)
        {
            if(relativePath == null)
            {
                throw new ArgumentNullException("relativePath");
            }

            if(transferState == null)
            {
                throw new ArgumentNullException("transferState");
            }

            this.m_RelativePath = relativePath;
            this.m_NormalizedRelativePath = GetNormalizedRelativePath(m_RelativePath);
            this.m_TransferState = transferState;
        }

        #endregion


        #region Overrides

        public override int GetHashCode()
        {
            return m_NormalizedRelativePath.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if(!(obj is IFileItem))
            {
                return false;
            }

            var other = obj as IFileItem;
            return this.m_NormalizedRelativePath.Equals(GetNormalizedRelativePath(other.RelativePath), 
                                                        StringComparison.InvariantCultureIgnoreCase)  &&
                this.CompareState == other.CompareState &&
                this.TransferState.Equals(other.TransferState);
        }

        #endregion


        #region Private Implementation

        string GetNormalizedRelativePath(string path)
        {
            path = path.Trim();

            path = path.Replace("\\", "/");
            while(path.Contains("//"))
            {
                path = path.Replace("//", "/");
            }

            return path;

        }

        #endregion
    }
}
