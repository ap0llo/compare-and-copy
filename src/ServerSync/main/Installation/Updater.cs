﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Squirrel;

namespace ServerSync.Installation
{
    class Updater
    {
        const string s_LastUpdateTimeStampFileName = "lastUpdate.timestamp";

        readonly UpdateOptions m_Options;
        Task m_UpdateTask;


        public UpdaterStatus Status { get; private set; } = UpdaterStatus.Initialized;
        
        public string Error { get; private set; }


        public Updater([NotNull] UpdateOptions options)
        {
            m_Options = options ?? throw new ArgumentNullException(nameof(options));            
        }

        public void Start()
        {
            Status = UpdaterStatus.Running;

            if (CanUpdate())
            {
                m_UpdateTask = StartUpdateTask();
            }
            else
            {
                m_UpdateTask = Task.CompletedTask;
            }

            m_UpdateTask.ContinueWith(t =>
            {
                if (!t.IsFaulted)
                    Status = UpdaterStatus.Completed;
            });
        }
        
        public void Stop()
        {
            try
            {
                m_UpdateTask.Wait();
                Status = UpdaterStatus.Completed;
            }
            catch (AggregateException aggregateException)
            {
                Error = aggregateException
                    .Flatten()
                    .InnerExceptions
                    .Select(e => e.Message)
                    .Aggregate((a, b) => a + "\n" + b);

                Status = UpdaterStatus.Failed;
            }
        }

        
        bool CanUpdate()
        {
            return ApplicationInfo.IsInstalled &&
                   m_Options.Enable &&
                   m_Options.Source != UpdateSource.NotConfigured &&
                   !String.IsNullOrEmpty(m_Options.Path);
        }
        
        async Task StartUpdateTask()
        {
            var lastUpdateTime = GetLastUpdateTime();
            if (lastUpdateTime.HasValue && (DateTime.UtcNow - lastUpdateTime.Value) < m_Options.Interval)
            {
                return;
            }
            
            switch (m_Options.Source)
            {
                case UpdateSource.NotConfigured:
                    throw new InvalidOperationException();

                case UpdateSource.GitHub:
                    await StartGitHubUpdateTask();
                    break;


                case UpdateSource.FileSystem:
                    await StartFileSystemUpdateTask();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }


            SetLastUpdateTime();
        }

        async Task StartFileSystemUpdateTask()
        {        
            using (var updateManager = new UpdateManager(m_Options.Path))
            {                
                await updateManager.UpdateApp();                
            }
        }
        
        async Task StartGitHubUpdateTask()
        {
            using (var updateManager = await UpdateManager.GitHubUpdateManager(
                repoUrl: m_Options.Path, 
                prerelease:m_Options.InstallPreReleaseVersions))
            {
                await updateManager.UpdateApp();
            }
        }

        DateTime? GetLastUpdateTime()
        {
            var file = GetLastUpdateTimeStampFile();

            if (file.Exists)
            {
                return file.LastWriteTimeUtc;
            }
            else
            {
                return null;
            }
        }

        void SetLastUpdateTime()
        {
            var file = GetLastUpdateTimeStampFile();

            if (!file.Exists)
            {
                using (file.Create()) { }
            }

            file.LastWriteTimeUtc = DateTime.UtcNow;
            file.Refresh();
        }

        FileInfo GetLastUpdateTimeStampFile()
        {
            var path = Path.Combine(ApplicationInfo.RootDirectory, s_LastUpdateTimeStampFileName);
            var fileInfo = new FileInfo(path);
            return fileInfo;
        }
    }

}