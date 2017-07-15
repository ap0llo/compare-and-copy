using System;
using System.IO;

namespace CompareAndCopy.Core.PathResolving
{
    class PathResolver : IPathResolver
    {
        readonly string m_BasePath;


        public PathResolver(string basePath)
        {
            m_BasePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
        }
        

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
    }
}
