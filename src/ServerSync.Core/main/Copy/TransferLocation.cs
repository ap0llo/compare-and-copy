using ServerSync.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ByteSizeLib;

namespace ServerSync.Core.Copy
{
    public class TransferLocation : ITransferLocation
    {

        #region Fields

        readonly string m_Name;
        readonly string m_Path;
        readonly ByteSize? m_MaximumSize;

        #endregion

        #region Properties

        public string Name { get { return m_Name; } }

        public string RootPath { get { return m_Path; } }

        public ByteSize? MaximumSize { get { return m_MaximumSize; } }

        #endregion


        #region Constructor

        public TransferLocation(string name, string path, ByteSize? maximumSize)
        {

            if(name == null)
            {
                throw new ArgumentNullException("name"); ;
            }

            if(path == null)
            {
                throw new ArgumentNullException("path");
            }

            if(String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("'name' must not be empty");
            }

            this.m_Name = name;
            this.m_Path = path;
            this.m_MaximumSize = maximumSize;
        }

        #endregion

    }
}
