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

        #region Private Type

        /// <summary>
        /// Class used for caching directory sizes in GetDirectorySize()
        /// </summary>
        private class ByteSizeCacheEntry
        {
            /// <summary>
            /// The size to be cached
            /// </summary>
            public ByteSize.ByteSize Value { get; set; }

            /// <summary>
            /// The time the cache entry was created
            /// </summary>
            public DateTime Created { get; set; }
        }

        #endregion


        #region Constants

        //the amount of time a value is considered up to date in the cache used by GetDirectorySize()
        static readonly TimeSpan s_CacheTime = new TimeSpan(0, 0, 5);

        #endregion


        #region Fields

        //cache used by GetDirectorySize() to speed up determining the size of a directoy
        static Dictionary<String, ByteSizeCacheEntry> s_GetDirectorySizeCache = new Dictionary<string, ByteSizeCacheEntry>();

        #endregion


        #region Public Methods

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
        /// <param name="path">The path of the directory to determine the size of</param>
        /// <param name="useCache">Enables or disables the cache. If set to false, the size of the directory will not be 
        /// looked up in the cache nor will the determined size be written to the cache </param>
        public static ByteSize.ByteSize GetDirectorySize(string path, bool useCache = true)
        {
            //if enabled, look up the size of the directory in the cache
            var cacheKey = path.ToLower().Trim();
            lock(s_GetDirectorySizeCache)
            {
                if(useCache && s_GetDirectorySizeCache.ContainsKey(cacheKey))
                {
                    var cacheEntry = s_GetDirectorySizeCache[cacheKey];

                    //determine if size from cache is recent enough
                    if((DateTime.Now - cacheEntry.Created) < s_CacheTime)
                    {
                        //return cache-value
                        return cacheEntry.Value;
                    }
                    else
                    {
                        //remove outdated value from cache
                        s_GetDirectorySizeCache.Remove(cacheKey);
                    }
                }
            }


            //determine size of directory

            var result = ByteSize.ByteSize.FromBytes(0);

            foreach(var file in Directory.GetFiles(path))
            {
                var fileSize = new FileInfo(file).Length;
                result = result.AddBytes(fileSize);
            }

            foreach (var directory in Directory.GetDirectories(path))
            {
                var dirSize = GetDirectorySize(directory);
                result += dirSize;
            }


            //if enabled, store the directory's size in the cache
            if(useCache)
            {
                lock (s_GetDirectorySizeCache)
                {
                    if (!s_GetDirectorySizeCache.ContainsKey(cacheKey))
                    {
                        s_GetDirectorySizeCache.Add(cacheKey, new ByteSizeCacheEntry() { Created = DateTime.Now, Value = result });
                    }
                    else
                    {
                        s_GetDirectorySizeCache[cacheKey].Created = DateTime.Now;
                        s_GetDirectorySizeCache[cacheKey].Value = result;
                    }
                }
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

        #endregion

    }
}
