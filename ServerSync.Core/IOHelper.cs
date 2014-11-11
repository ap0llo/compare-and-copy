using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    public class IOHelper
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

        public static string GetRelativePath(string absolutePath, string relativeTo, bool relativeToIsDirectory)
        {
            if (relativeToIsDirectory)
            {
                relativeTo = Path.Combine(relativeTo, "dummy");
            }

            Uri fileUri = new Uri(absolutePath);
            Uri relativeToUri = new Uri(relativeTo);

            var resultUri = relativeToUri.MakeRelativeUri(fileUri);
            return Uri.UnescapeDataString(resultUri.ToString());
        }

        /// <summary>
        /// Gets the combined size of all files in the specified directory and any of its subdirectories
        /// </summary>
        public static ByteSize.ByteSize GetDirectorySize(string path)
        {
            var result = ByteSize.ByteSize.FromBytes(0);

            foreach(var file in Directory.GetFiles(path))
            {
                var fileSize = new FileInfo(file).Length;
                result.AddBytes(fileSize);
            }

            foreach (var directory in Directory.GetDirectories(path))
            {
                var dirSize = GetDirectorySize(directory);
                result += dirSize;
            }

            return result;
        }


        public static bool CopyFile(string sourcePath, string destinationPath)
        {

            var tmpPath = destinationPath + ".tmp";
            
            if(File.Exists(tmpPath))
            {
                Console.WriteLine("Error copying file '{0}' to '{1}': TmpFile already exists", sourcePath, destinationPath);
                return false;
            }

            try
            {
                EnsureDirectoryExists(Path.GetDirectoryName(destinationPath));

                File.Copy(sourcePath, tmpPath);
                File.Move(tmpPath, destinationPath);

            }
            catch (IOException ex)
            {

                Console.WriteLine("Error copying file '{0}' to '{1}': {2}", sourcePath, destinationPath, ex);
                return false;
            }
            finally
            {
                if(File.Exists(tmpPath))
                {
                    File.Delete(tmpPath);
                }
            }


            return true;
        }


        

    }
}
