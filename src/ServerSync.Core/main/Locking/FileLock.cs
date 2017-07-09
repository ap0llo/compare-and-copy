using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ServerSync.Core.Locking
{
    class FileLock
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        string m_LockFilePath;
        bool m_IsLocked;
        FileStream m_FileStream;


        /// <summary>
        /// The path to the file being locked
        /// </summary>
        public string LockFilePath
        {
            get
            {
                lock(this)
                {
                    return m_LockFilePath;
                }
            }
            private set
            {
                lock(this)
                {
                    m_LockFilePath = value;
                }
            }
        }

        /// <summary>
        /// Indicates whether the file is currently locked by this instance of FileLock
        /// </summary>
        public bool IsLocked
        {
            get
            {
                lock(this)
                {
                    return m_IsLocked;
                }
            }
            private set
            {
                lock(this)
                {
                    m_IsLocked = value;
                }
            }
        }


        public FileLock(string lockFilePath)
        {
            if(String.IsNullOrEmpty(lockFilePath))
                throw new ArgumentNullException(nameof(lockFilePath));

            LockFilePath = lockFilePath;
        }


        /// <summary>
        /// Locks the file. 
        /// Suspends the current thread until a lock can be acquired.
        /// </summary>
        /// <returns>Returns whether the file lock has been acquired</returns>
        public bool Lock()
        {
            m_Logger.Info("Trying to acquire lock with lock-file {0}", this.LockFilePath);
            var task = Task.Factory.StartNew(() => LockInternal(null));
            task.Wait();
            m_Logger.Info("Lock on file {0} acquired", this.LockFilePath);
            return true;
        }

        /// <summary>
        /// Locks the file.
        /// Suspends the current thread until a lock is required or the specified time has elapsed
        /// </summary>
        /// <returns>Returns whether the file lock has been acquired</returns>
        public bool Lock(TimeSpan timeout)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            m_Logger.Info("Trying to acquire lock with lock-file {0} and timeout {1}", this.LockFilePath, timeout);
            var task = Task.Factory.StartNew(() => LockInternal(cancellationTokenSource), cancellationTokenSource.Token);
            task.Wait((int) timeout.TotalMilliseconds);

            if(task.IsCompleted)
            {
                m_Logger.Info("Lock on file {0} acquired", this.LockFilePath);
                return true;
            }
            else
            {
                cancellationTokenSource.Cancel();               
                m_Logger.Info("Failed to acquired lock for lock-file {0}", this.LockFilePath);

                return false;
            }
        }
        
        /// <summary>
        /// Unlocks the file if it is currently locked
        /// </summary>
        public void Unlock()
        {
            lock(this)
            {
                if(!IsLocked)
                {
                    return;
                }

                //dispose FileStream that was used to lock the file
                m_FileStream.Close();
                m_FileStream = null;


                this.IsLocked = false;
            }
        }


        void LockInternal(CancellationTokenSource cancellationTokenSource)
        {
            lock (this)
            {
                //file is already locked => nothing to do
                if (IsLocked)
                {
                    return;
                }

                Directory.CreateDirectory(Path.GetDirectoryName(this.LockFilePath));

                while (true)
                {
                    //try to lock the file
                    try
                    {
                        m_FileStream = File.Open(this.LockFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                        break;
                    }
                    catch (IOException)
                    {
                        //file could not be locked => suspend thread, try again later
                        try
                        {
                            if(cancellationTokenSource == null)
                            {
                                Task.Delay(5000).Wait();
                            }
                            else
                            {
                                Task.Delay(5000).Wait(cancellationTokenSource.Token);    
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            //operation was canceled => break out of loop
                            m_Logger.Info("LockInternal(): The Task was canceled");
                            break;
                        }
                    }
                }

                //set new lock state
                this.IsLocked = true;
            }
        }
    }
}
