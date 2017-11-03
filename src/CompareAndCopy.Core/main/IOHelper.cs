using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ByteSizeLib;
using System.Threading;

namespace CompareAndCopy.Core
{
    public class IOHelper
    {
        /// <summary>
        /// Class used for caching directory sizes in GetDirectorySize()
        /// </summary>
        private class ByteSizeCacheEntry
        {
            /// <summary>
            /// The size to be cached
            /// </summary>
            public ByteSize Value { get; set; }

            /// <summary>
            /// The time the cache entry was created
            /// </summary>
            public DateTime Created { get; set; }
        }


        //the amount of time a value is considered up to date in the cache used by GetDirectorySize()
        static readonly TimeSpan s_CacheTime = TimeSpan.FromSeconds(5);
        const int s_NumberOfRetries = 2;
        static readonly TimeSpan s_RetryDelay = TimeSpan.FromSeconds(3);

        //cache used by GetDirectorySize() to speed up determining the size of a directory
        static Dictionary<String, ByteSizeCacheEntry> s_GetDirectorySizeCache = new Dictionary<string, ByteSizeCacheEntry>();

        static readonly Logger s_Logger = LogManager.GetCurrentClassLogger();
   

        public static string GetRelativePath(string absolutePath, string relativeTo, bool relativeToIsDirectory)
        {
            if (relativeToIsDirectory)
            {
                relativeTo = Path.Combine(relativeTo, "dummy");
            }

            var fileUri = new Uri(absolutePath);
            var relativeToUri = new Uri(relativeTo);

            var resultUri = relativeToUri.MakeRelativeUri(fileUri);
            return Uri.UnescapeDataString(resultUri.ToString());
        }

        /// <summary>
        /// Gets the combined size of all files in the specified directory and any of its subdirectories
        /// </summary>
        /// <param name="path">The path of the directory to determine the size of</param>
        /// <param name="useCache">Enables or disables the cache. If set to false, the size of the directory will not be 
        /// looked up in the cache nor will the determined size be written to the cache </param>
        public static ByteSize GetDirectorySize(string path, bool useCache = true)
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

            var result = ByteSize.FromBytes(0);

            string[] files;            
            try
            {
                files = GetFiles(path);                
            }
            catch (UnauthorizedAccessException)
            {
                s_Logger.Error($"Could not get files from directory {path}. Assuming an empty directory");
                files = new string[0];
            }

            foreach (var file in files)
            {
                var fileSize = new FileInfo(file).Length;
                result = result.AddBytes(fileSize);
            }

            string[] dirs;
            try
            {
                dirs = GetDirectories(path);
            }
            catch (UnauthorizedAccessException)
            {
                s_Logger.Error($"Could not get directories from directory {path}. Assuming an empty directory");
                dirs = new string[0];
            }

            foreach (var directory in dirs)
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
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

                File.Copy(sourcePath, tmpPath, true);

                if(File.Exists(destinationPath))
                {
                    File.Delete(destinationPath);
                }

                File.Move(tmpPath, destinationPath);

            }
            catch (IOException ex)
            {
                s_Logger.Error("Could not copy file '{0}' to '{1}': {2}", sourcePath, destinationPath, ex);
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

        /// <summary>
        /// Determines if the specified relative path, when combined with the specified root path,
        /// references a file that is not within the root path
        /// </summary>
        public static bool PathLeavesRoot(string rootPath, string relativePath)
        {
            if(rootPath == null)
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            if(relativePath == null)
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            if(String.IsNullOrWhiteSpace(rootPath))
            {
                rootPath = ".";
            }

            rootPath = Path.GetFullPath(rootPath);
            string absolutePath;

            if (Path.IsPathRooted(relativePath))
            {
                absolutePath = relativePath;
            }
            else
            {
                absolutePath = Path.Combine(rootPath, relativePath);
            }
            absolutePath = Path.GetFullPath(absolutePath);

            return !absolutePath.StartsWith(rootPath, StringComparison.InvariantCultureIgnoreCase);
        }

        public static IEnumerable<string> GetAllFilesRelative(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return Enumerable.Empty<string>();
            }
            else
            {
                var allFiles = GetFiles(directory, "*", SearchOption.AllDirectories);
                return allFiles.Select(path => GetRelativePath(path, directory, true));
            }
        }

        public static string[] GetDirectories(string path) => 
            ExecuteWithRetries($"Getting directories in '{path}'", () => Directory.GetDirectories(path));
        
        public static string[] GetFiles(string path) => 
            ExecuteWithRetries($"Getting files for in '{path}'", () => Directory.GetFiles(path));

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption) =>
            ExecuteWithRetries($"Getting files for in '{path}'", () => Directory.GetFiles(path, searchPattern, searchOption));
        
        static T ExecuteWithRetries<T>(string logMessage, Func<T> action)
        {
            s_Logger.Debug(logMessage);

            var remainingRetries = s_NumberOfRetries;
            while(true)
            {                
                try
                {
                    return action();
                }
                catch (IOException) when (remainingRetries > 0)
                {
                    s_Logger.Warn($"Caught IOExeption thrown by operation '{logMessage}', retrying");
                    Thread.Sleep(s_RetryDelay);
                    remainingRetries -= 1;
                }                
            }            
        }
    }
}
