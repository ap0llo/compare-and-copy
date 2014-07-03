using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    class IOHelper
    {
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                var parent = Path.GetDirectoryName(path);
                EnsureDirectoryExists(parent);

                Directory.CreateDirectory(path);
            }
        }
    }
}
