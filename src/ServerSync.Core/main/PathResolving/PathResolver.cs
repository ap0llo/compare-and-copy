using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.PathResolving
{
    class PathResolver : IPathResolver
    {

        #region Fields

        readonly string m_BasePath;

        #endregion


        #region Constructor

        public PathResolver(string basePath)
        {
            if (basePath == null)
            {
                throw new ArgumentNullException("basePath");
            }

            this.m_BasePath = basePath;
        }
        
        #endregion


        #region IPathResolver Implementation

        public string GetAbsolutePath(string inputPath)
        {
            string result = null;
            if(Path.IsPathRooted(inputPath))
            {
                result = inputPath;
            }
            else
            {
                result = Path.Combine(m_BasePath, inputPath);
            }
            return Path.GetFullPath(result);
        }

        #endregion

    }
}
