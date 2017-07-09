using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.PathResolving
{
    public interface IPathResolver
    {
        string GetAbsolutePath(string inputPath);
    }
}
