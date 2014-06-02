using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSync.Core.Configuration;

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

        public FolderComparisonResult Run()
        {
            filesMissingLeft = new List<string>();
            filesMissingRight = new List<string>();
            conflicts = new List<string>();
            sameFiles = new List<string>();

            CompareFolders("");

            return new FolderComparisonResult(filesMissingLeft, filesMissingRight, conflicts, sameFiles);
        }

        #endregion


        #region Private Implementation

        private void CompareFolders(string relativePath)
        {
            Console.WriteLine("Scanning {0}", relativePath);

            var leftAbsoulutePath = Path.Combine(config.Left.RootPath, relativePath);
            var rightAbsolutePath = Path.Combine(config.Right.RootPath, relativePath);

            //compare directories
            var leftDirectories = Directory.GetDirectories(leftAbsoulutePath)
                                           .Where(ApplyIncludeDirectoriesFilter)
                                           .Select(path => Path.GetFileName(path));

            var rightDirectories = Directory.GetDirectories(rightAbsolutePath)
                                            .Where(ApplyIncludeDirectoriesFilter)
                                            .Select(path => Path.GetFileName(path));

            //get absolute paths of directories only found in the left folder
            var uniqueLeft = leftDirectories.Where(lName => !rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                            .Select(name => Path.Combine(config.Left.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.filesMissingRight.AddRange(uniqueLeft.SelectMany(dir => GetAllFiles(dir)).Select(fullPath => GetRelativePath(fullPath, config.Left.RootPath, true)));

            //get absolute paths of directories only found in the right folder
            var uniqueRight = rightDirectories.Where(rName => !leftDirectories.Contains(rName, StringComparer.InvariantCultureIgnoreCase))
                                              .Select(name => Path.Combine(config.Right.RootPath, relativePath, name));

            //add all files in the subtree to the list of files only found in one location
            this.filesMissingLeft.AddRange(uniqueRight.SelectMany(dir => GetAllFiles(dir)).Select(fullPath => GetRelativePath(fullPath, config.Right.RootPath, true)));


            //compare directories found on both sides
            var sameDirectories = leftDirectories.Where(lName => rightDirectories.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                                 .Select(name => Path.Combine(relativePath, name));
            foreach (var item in sameDirectories)
            {                
                CompareFolders(item);
            }

            //compare files
            var filesLeft = Directory.GetFiles(leftAbsoulutePath)
                                     .Select(path => Path.GetFileName(path))
                                     .Where(name => !config.ExcludedFiles.Any(regex => regex.IsMatch(Path.Combine(config.Left.RootPath, relativePath, name)))); 

            var filesRight = Directory.GetFiles(rightAbsolutePath)
                                      .Select(path => Path.GetFileName(path))
                                      .Where(name => !config.ExcludedFiles.Any(regex => regex.IsMatch(Path.Combine(config.Right.RootPath, relativePath, name)))); 


            filesMissingRight.AddRange(filesLeft.Where(lName => !filesRight.Contains(lName, StringComparer.InvariantCultureIgnoreCase))
                                                .Select(name => Path.Combine(relativePath, name)));

            filesMissingLeft.AddRange(filesRight.Where(name => !filesLeft.Contains(name, StringComparer.InvariantCultureIgnoreCase))
                                                .Select(name => Path.Combine(relativePath, name)));


            //compare files that exist in both left and right directories
            var sameFiles = filesLeft.Where(name => filesRight.Contains(name, StringComparer.InvariantCultureIgnoreCase))
                                     .Select(name => Path.Combine(relativePath, name));

            foreach (var filePath in sameFiles)
            {
                var leftAbsoluteFilePath = Path.Combine(config.Left.RootPath, filePath);
                var rightAbsoluteFilePath = Path.Combine(config.Right.RootPath, filePath);

                if (!FilesAreEqual(filePath))
                {
                    this.conflicts.Add(filePath);
                }
                else
                {
                    this.sameFiles.Add(filePath);
                }                
            }

        }


        private IEnumerable<string> GetAllFiles(string dirAbsoultePath)
        {

            Console.WriteLine("Scanning {0}", dirAbsoultePath);

            if(!ApplyIncludeDirectoriesFilter(dirAbsoultePath))
            {
                return Enumerable.Empty<string>();
            }

            var childFiles = Directory.GetDirectories(dirAbsoultePath).SelectMany(dir => GetAllFiles(dir));
            var allFiles = Directory.GetFiles(dirAbsoultePath).Union(childFiles);

            //apply filter
            return allFiles.Where(filePath => !config.ExcludedFiles.Any(regex => regex.IsMatch(filePath)));
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


        private bool ApplyIncludeDirectoriesFilter(string path)
        {
            if(config.IncludeFolders.Any())
            {
                return config.IncludeFolders.Any(regex => regex.IsMatch(path));
            }
            else
            {
                return true;
            }
        }

        #endregion Private Implementation
    
    }
}
