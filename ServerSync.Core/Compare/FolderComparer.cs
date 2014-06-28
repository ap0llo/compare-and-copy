﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;

namespace ServerSync.Core.Compare
{
    public class FolderComparer
    {

        #region Fields

        private SyncConfiguration config;

        private List<string> filesMissingLeft = new List<string>();
        private List<string> filesMissingRight = new List<string>();
        private List<string> conflicts = new List<string>();
        private List<string> sameFiles = new List<string>();

        #endregion Fields


        #region Constructor

        public FolderComparer(SyncConfiguration config)
        {
            this.config = config;
        }

        #endregion Constructor


        #region Public Methods

        public SyncState Run()
        {
            filesMissingLeft = new List<string>();
            filesMissingRight = new List<string>();
            conflicts = new List<string>();
            sameFiles = new List<string>();

            CompareFolders("");


            var files = filesMissingLeft.Select(path => new FileItem() { RelativePath = path, CompareState = CompareState.MissingLeft });
            files = files.Union(filesMissingRight.Select(path => new FileItem() { RelativePath = path, CompareState = CompareState.MissingRight }));
            files = files.Union(conflicts.Select(path => new FileItem() { RelativePath = path, CompareState = CompareState.Conflict }));

            return new SyncState(files.ToList());
        }

        #endregion


        #region Private Implementation

        private void CompareFolders(string relativePath)
        {            

            var leftAbsoulutePath = Path.Combine(config.Left.RootPath, relativePath);
            var rightAbsolutePath = Path.Combine(config.Right.RootPath, relativePath);

            //compare directories
            var leftDirectories = Directory.GetDirectories(leftAbsoulutePath)                                           
                                           .Select(path => Path.GetFileName(path));

            var rightDirectories = Directory.GetDirectories(rightAbsolutePath)                                            
                                            .Select(path => Path.GetFileName(path));

            //get absolute paths of directories only found in the left folder
            var uniqueLeft = leftDirectories.Where(lName => !rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                            .Select(name => Path.Combine(config.Left.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.filesMissingRight.AddRange(uniqueLeft.SelectMany(dir => GetFiles(dir, true)).Select(fullPath => GetRelativePath(fullPath, config.Left.RootPath, true)));

            //get absolute paths of directories only found in the right folder
            var uniqueRight = rightDirectories.Where(rName => !leftDirectories.Contains(rName, StringComparer.InvariantCultureIgnoreCase))
                                              .Select(name => Path.Combine(config.Right.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.filesMissingLeft.AddRange(uniqueRight.SelectMany(dir => GetFiles(dir, true)).Select(fullPath => GetRelativePath(fullPath, config.Right.RootPath, true)));


            //compare directories found on both sides
            var sameDirectories = leftDirectories.Where(lName => rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                                 .Select(name => Path.Combine(relativePath, name));

            sameDirectories.AsParallel()
                           .WithDegreeOfParallelism(4)
                           .ForAll(CompareFolders);


            //compare files
            var filesLeft = GetFiles(leftAbsoulutePath, false)
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
            Console.WriteLine("Scanning {0}", dirAbsoultePath);


            var childFiles = recurse ?
                Directory.GetDirectories(dirAbsoultePath).SelectMany(dir => GetFiles(dir, recurse)).Select(relPath => Path.Combine(dirAbsoultePath, relPath)) :
                Enumerable.Empty<string>();

            var allFiles = Directory.GetFiles(dirAbsoultePath).Union(childFiles);


            //apply filter
            return allFiles; 
        }

        private static string GetRelativePath(string absolutePath, string relativeTo, bool relativeToIsDirectory)
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

        private bool FilesAreEqual(string relativePath)
        {
            var info1 = new FileInfo(Path.Combine(config.Left.RootPath, relativePath));
            var info2 = new FileInfo(Path.Combine(config.Right.RootPath, relativePath));

            var sizeDifference = info1.Length - info2.Length;           
            var modifiedDifference = (info1.LastWriteTime - info2.LastWriteTime).TotalMilliseconds;

            return sizeDifference == 0 && Math.Abs(modifiedDifference) <= config.TimeStampMargin;
        }


        


        #endregion Private Implementation
    
    }
}
