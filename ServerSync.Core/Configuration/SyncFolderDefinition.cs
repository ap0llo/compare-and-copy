using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration
{
    public class SyncFolderDefinition : ISyncFolderDefinition
    {
        #region Fields

        readonly string m_Name;
        readonly string m_RootPath;


        #endregion


        #region Properties

        public string Name { get { return m_Name; } }

        public string RootPath { get { return m_RootPath; } }

        #endregion


        #region Constructor
        
        public SyncFolderDefinition(string name, string rootPath)
        {
            if(name == null)
            {
                throw new ArgumentNullException("name");
            }

            if(rootPath == null)
            {
                throw new ArgumentNullException("rootPath");
            }


            if(String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("'name' must not be empty");
            }

            this.m_Name = name;
            this.m_RootPath = rootPath;
        }

        #endregion

    }
}
