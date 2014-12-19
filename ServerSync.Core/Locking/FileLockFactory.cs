using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Locking
{
    class FileLockFactory
    {
        #region Fields

        //cache for FileLock objects (there must only be one instance per file)
        static Dictionary<string, FileLock> s_FileLocks = new Dictionary<string, FileLock>();

        //lock object to synchronize concurrent calls to the factory
        static object s_Lock = new object();

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the FileLock instance for the specified file that can be used to lock/unlock the file
        /// </summary>
        public static FileLock GetFileLock(string lockFileName)
        {
            lock(s_Lock)
            {
                var key = GetFileLockKey(lockFileName);

                //no instance in cache => create new instance
                if(!s_FileLocks.ContainsKey(key))
                {
                    s_FileLocks.Add(key, new FileLock(lockFileName));
                }

                //return proper FileLock instance
                return s_FileLocks[key];
            }            
        }

        #endregion


        #region Private Methods       

        static string GetFileLockKey(string lockFileName)
        {
            return lockFileName.ToLower().Trim().Replace("/", "\\");
        }

        #endregion
    }
}
