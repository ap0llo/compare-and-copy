

using System;

namespace ServerSync.Installation
{
    interface IInstallerStep
    {
        void OnInitialInstall(Version version);

        void OnAppUpdate(Version version);

        void OnAppUninstall(Version version);
    }
}