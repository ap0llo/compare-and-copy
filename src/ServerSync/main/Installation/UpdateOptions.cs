using System;

namespace ServerSync.Installation
{
    sealed class UpdateOptions
    {
        public bool Enable { get; set; } = false;

        public UpdateSource Source { get; set; } = UpdateSource.NotConfigured;

        public string Path { get; set; } = "";

        public bool InstallPreReleaseVersions { get; set; } = false;

        public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
    }    
}