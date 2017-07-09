using ServerSync.Model.State;
using System;

namespace ServerSync.Core.State
{
    public class FileItem : IFileItem
    {
        readonly string m_NormalizedRelativePath;
        

        public string RelativePath { get; }

        public CompareState CompareState { get; set; }

        public TransferState TransferState { get; }


        public FileItem(string relativePath) : this(relativePath, new TransferState())
        {
        }

        public FileItem(string relativePath, TransferState transferState)
        {
            RelativePath = relativePath ?? throw new ArgumentNullException(nameof(relativePath));
            TransferState = transferState ?? throw new ArgumentNullException(nameof(transferState));
            m_NormalizedRelativePath = GetNormalizedRelativePath(RelativePath);
        }


        public override int GetHashCode() => m_NormalizedRelativePath.ToLower().GetHashCode();

        public override bool Equals(object obj)
        {
            if(obj is IFileItem other)
            {
                return m_NormalizedRelativePath.Equals(GetNormalizedRelativePath(other.RelativePath), StringComparison.InvariantCultureIgnoreCase) &&
                       CompareState == other.CompareState &&
                       TransferState.Equals(other.TransferState); 
            }

            return false;
        }


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
    }
}
