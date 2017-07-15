using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NLog;
using CompareAndCopy.Core.State;
using CompareAndCopy.Model.Configuration;
using CompareAndCopy.Model.State;

namespace CompareAndCopy.Core.Compare
{
    /// <summary>
    /// Implementation of comparison between two folders
    /// </summary>
    public class FolderComparer
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        readonly ISyncConfiguration m_Config;
		readonly TimeSpan m_TimeStampMargin;

        List<string> m_FilesMissingLeft = new List<string>();
        List<string> m_FilesMissingRight = new List<string>();
        List<string> m_Conflicts = new List<string>();
        List<string> m_SameFiles = new List<string>();

        
        /// <summary>
        /// Initializes a new instance of FolderComparer
        /// </summary>
        /// <param name="config">The configuration to use for comparison</param>
		/// <param name="timeStampMargin">The maximum time span by which two file timestamps may differ to be considered equal</param>
        public FolderComparer(ISyncConfiguration config, TimeSpan timeStampMargin)
        {
            m_Config = config ?? throw new ArgumentNullException(nameof(config));
			m_TimeStampMargin = timeStampMargin;
        }
        

        public ISyncState Run()
        {
            //clear lists
            m_FilesMissingLeft = new List<string>();
            m_FilesMissingRight = new List<string>();
            m_Conflicts = new List<string>();
            m_SameFiles = new List<string>();

            if(!Directory.Exists(this.m_Config.Left.RootPath))
            {
                m_Logger.Error("Left root directory does not exist");
                return null;
            }

            if (!Directory.Exists(this.m_Config.Right.RootPath))
            {
                m_Logger.Error("Right root directory does not exist");
                return null;
            }


            //run comparison
            CompareFolders("");

            //build SyncState object from file lists
            var files = m_FilesMissingLeft.Select(path => new FileItem(path) 
                { 
                    CompareState = CompareState.MissingLeft 
                });
            files = files.Union(m_FilesMissingRight.Select(path => new FileItem(path) 
                { 
                    CompareState = CompareState.MissingRight 
                }));
            files = files.Union(m_Conflicts.Select(path => new FileItem(path) 
                {  
                    CompareState = CompareState.Conflict 
                }));

            return new SyncState(files.ToList());
        }


        void CompareFolders(string relativePath)
        {            

            var leftAbsolutePath = Path.Combine(m_Config.Left.RootPath, relativePath);
            var rightAbsolutePath = Path.Combine(m_Config.Right.RootPath, relativePath);

            if(!Directory.Exists(leftAbsolutePath))
            {
                m_FilesMissingLeft.AddRange(GetFiles(rightAbsolutePath, true).Select(absPath => IOHelper.GetRelativePath(absPath, rightAbsolutePath, true)));
                return;
            }


            if(!Directory.Exists(rightAbsolutePath))
            {
                m_FilesMissingRight.AddRange(GetFiles(leftAbsolutePath, true).Select(absPath => IOHelper.GetRelativePath(absPath, leftAbsolutePath, true)));
                return;
            }

            //compare directories
            var leftDirectories = Directory.GetDirectories(leftAbsolutePath)                                           
                                           .Select(path => Path.GetFileName(path));

            var rightDirectories = Directory.GetDirectories(rightAbsolutePath)                                            
                                            .Select(path => Path.GetFileName(path));

            //get absolute paths of directories only found in the left folder
            var uniqueLeft = leftDirectories.Where(lName => !rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                            .Select(name => Path.Combine(m_Config.Left.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.m_FilesMissingRight.AddRange(uniqueLeft.SelectMany(dir => GetFiles(dir, true)).Select(fullPath => IOHelper.GetRelativePath(fullPath, m_Config.Left.RootPath, true)));

            //get absolute paths of directories only found in the right folder
            var uniqueRight = rightDirectories.Where(rName => !leftDirectories.Contains(rName, StringComparer.InvariantCultureIgnoreCase))
                                              .Select(name => Path.Combine(m_Config.Right.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.m_FilesMissingLeft.AddRange(uniqueRight.SelectMany(dir => GetFiles(dir, true)).Select(fullPath => IOHelper.GetRelativePath(fullPath, m_Config.Right.RootPath, true)));


            //compare directories found on both sides
            var sameDirectories = leftDirectories.Where(lName => rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                                 .Select(name => Path.Combine(relativePath, name));

            sameDirectories.AsParallel()
                           .WithDegreeOfParallelism(4)
                           .ForAll(CompareFolders);


            //compare files
            var filesLeft = GetFiles(leftAbsolutePath, false).Select(Path.GetFileName);
            var filesRight = GetFiles(rightAbsolutePath, false).Select(Path.GetFileName);
                             
            lock(this)
            {

                m_FilesMissingRight.AddRange(filesLeft.Where(lName => !filesRight.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                                    .Select(name => Path.Combine(relativePath, name)));

                m_FilesMissingLeft.AddRange(filesRight.Where(name => !filesLeft.Contains(name, StringComparer.InvariantCultureIgnoreCase))
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
                        m_Conflicts.Add(filePath);
                    }
                }
                else
                {
                    lock(this)
                    {
                        m_SameFiles.Add(filePath);
                    }
                }           
            }            
        }

        IEnumerable<string> GetFiles(string dirAbsoultePath, bool recurse)
        {
            m_Logger.Info("Scanning {0}", dirAbsoultePath);

            var childFiles = recurse 
                ? Directory.GetDirectories(dirAbsoultePath).SelectMany(dir => GetFiles(dir, true)).Select(relPath => Path.Combine(dirAbsoultePath, relPath)) 
                : Enumerable.Empty<string>();

            var allFiles = Directory.GetFiles(dirAbsoultePath).Union(childFiles);

            //apply filter
            return allFiles; 
        }
        
        bool FilesAreEqual(string relativePath)
        {
            var info1 = new FileInfo(Path.Combine(m_Config.Left.RootPath, relativePath));
            var info2 = new FileInfo(Path.Combine(m_Config.Right.RootPath, relativePath));

            var sizeDifference = info1.Length - info2.Length;           
            var modifiedDifference = (info1.LastWriteTime - info2.LastWriteTime).TotalMilliseconds;

            return sizeDifference == 0 && Math.Abs(modifiedDifference) <= m_TimeStampMargin.TotalMilliseconds;
        }        
    }
}
