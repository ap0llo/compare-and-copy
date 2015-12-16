using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using NLog;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;

namespace ServerSync.Core.Compare
{
    /// <summary>
    /// Implementation of comparison between two folders
    /// </summary>
    public class FolderComparer
    {

        #region Fields

        Logger m_Logger = LogManager.GetCurrentClassLogger();


        readonly ISyncConfiguration config;
		readonly TimeSpan timeStampMargin;

        private List<string> filesMissingLeft = new List<string>();
        private List<string> filesMissingRight = new List<string>();
        private List<string> conflicts = new List<string>();
        private List<string> sameFiles = new List<string>();

        #endregion Fields


        #region Constructor

        /// <summary>
        /// Initializes a new instance of FolderComparer
        /// </summary>
        /// <param name="config">The configuration to use for comparison</param>
		/// <param name="timeStampMargin">The maximum time span by which two file timestamps may differ to be considered equal</param>
        public FolderComparer(ISyncConfiguration config, TimeSpan timeStampMargin)
        {
			if (config == null) throw new ArgumentNullException("config");
			
            this.config = config;
			this.timeStampMargin = timeStampMargin;
        }

        #endregion Constructor


        #region Public Methods

        public ISyncState Run()
        {
            //clear lists
            filesMissingLeft = new List<string>();
            filesMissingRight = new List<string>();
            conflicts = new List<string>();
            sameFiles = new List<string>();

            if(!Directory.Exists(this.config.Left.RootPath))
            {
                m_Logger.Error("Left root directory does not exist");
                return null;
            }

            if (!Directory.Exists(this.config.Right.RootPath))
            {
                m_Logger.Error("Right root directory does not exist");
                return null;
            }


            //run comparison
            CompareFolders("");

            //build SyncState object from file lists
            var files = filesMissingLeft.Select(path => new FileItem(path) 
                { 
                    CompareState = CompareState.MissingLeft 
                });
            files = files.Union(filesMissingRight.Select(path => new FileItem(path) 
                { 
                    CompareState = CompareState.MissingRight 
                }));
            files = files.Union(conflicts.Select(path => new FileItem(path) 
                {  
                    CompareState = CompareState.Conflict 
                }));

            return new SyncState(files.ToList());
        }

        #endregion


        #region Private Implementation

        private void CompareFolders(string relativePath)
        {            

            var leftAbsolutePath = Path.Combine(config.Left.RootPath, relativePath);
            var rightAbsolutePath = Path.Combine(config.Right.RootPath, relativePath);

            if(!Directory.Exists(leftAbsolutePath))
            {
                filesMissingLeft.AddRange(GetFiles(rightAbsolutePath, true).Select(absPath => IOHelper.GetRelativePath(absPath, rightAbsolutePath, true)));
                return;
            }


            if(!Directory.Exists(rightAbsolutePath))
            {
                filesMissingRight.AddRange(GetFiles(leftAbsolutePath, true).Select(absPath => IOHelper.GetRelativePath(absPath, leftAbsolutePath, true)));
                return;
            }

            //compare directories
            var leftDirectories = Directory.GetDirectories(leftAbsolutePath)                                           
                                           .Select(path => Path.GetFileName(path));

            var rightDirectories = Directory.GetDirectories(rightAbsolutePath)                                            
                                            .Select(path => Path.GetFileName(path));

            //get absolute paths of directories only found in the left folder
            var uniqueLeft = leftDirectories.Where(lName => !rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                            .Select(name => Path.Combine(config.Left.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.filesMissingRight.AddRange(uniqueLeft.SelectMany(dir => GetFiles(dir, true)).Select(fullPath => IOHelper.GetRelativePath(fullPath, config.Left.RootPath, true)));

            //get absolute paths of directories only found in the right folder
            var uniqueRight = rightDirectories.Where(rName => !leftDirectories.Contains(rName, StringComparer.InvariantCultureIgnoreCase))
                                              .Select(name => Path.Combine(config.Right.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.filesMissingLeft.AddRange(uniqueRight.SelectMany(dir => GetFiles(dir, true)).Select(fullPath => IOHelper.GetRelativePath(fullPath, config.Right.RootPath, true)));


            //compare directories found on both sides
            var sameDirectories = leftDirectories.Where(lName => rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                                 .Select(name => Path.Combine(relativePath, name));

            sameDirectories.AsParallel()
                           .WithDegreeOfParallelism(4)
                           .ForAll(CompareFolders);


            //compare files
            var filesLeft = GetFiles(leftAbsolutePath, false)
                                     .Select(path => Path.GetFileName(path));


            var filesRight = GetFiles(rightAbsolutePath, false)
                                      .Select(path => Path.GetFileName(path));
                             

            lock(this)
            {

                filesMissingRight.AddRange(filesLeft.Where(lName => !filesRight.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                                    .Select(name => Path.Combine(relativePath, name)));

                filesMissingLeft.AddRange(filesRight.Where(name => !filesLeft.Contains(name, StringComparer.InvariantCultureIgnoreCase))
                                                    .Select(name => Path.Combine(relativePath, name)));
            }


            //compare files that exist in both left and right directories
            foreach(var name in filesLeft.Where(name => filesRight.Contains(name, StringComparer.InvariantCultureIgnoreCase)))
            {                                         
                var filePath = Path.Combine(relativePath, name);
                if (!FilesAreEqual(filePath))
                {
                    lock(this)
                    {
                        this.conflicts.Add(filePath);
                    }
                }
                else
                {
                    lock(this)
                    {
                        this.sameFiles.Add(filePath);
                    }
                }           
            };            
        }

        private IEnumerable<string> GetFiles(string dirAbsoultePath, bool recurse)
        {
            m_Logger.Info("Scanning {0}", dirAbsoultePath);


            var childFiles = recurse ?
                Directory.GetDirectories(dirAbsoultePath).SelectMany(dir => GetFiles(dir, recurse)).Select(relPath => Path.Combine(dirAbsoultePath, relPath)) :
                Enumerable.Empty<string>();

            var allFiles = Directory.GetFiles(dirAbsoultePath).Union(childFiles);


            //apply filter
            return allFiles; 
        }
        
        private bool FilesAreEqual(string relativePath)
        {
            var info1 = new FileInfo(Path.Combine(config.Left.RootPath, relativePath));
            var info2 = new FileInfo(Path.Combine(config.Right.RootPath, relativePath));

            var sizeDifference = info1.Length - info2.Length;           
            var modifiedDifference = (info1.LastWriteTime - info2.LastWriteTime).TotalMilliseconds;

            return sizeDifference == 0 && Math.Abs(modifiedDifference) <= timeStampMargin.TotalMilliseconds;
        }        

        #endregion Private Implementation
    
    }
}
