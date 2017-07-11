using System;
using System.IO;
using System.Reflection;

namespace ServerSync.Installation
{
    class ConfigFileInstallerStep : IInstallerStep
    {
        const string s_DefaultConfigResourceName = "ServerSync.config.json";


        string ConfigFilePath => Path.Combine(ApplicationInfo.RootDirectory, Configuration.ConfigFileName);


        public void OnInitialInstall(Version version)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(s_DefaultConfigResourceName))
            using (var reader = new StreamReader(stream))
            {
                var content = reader.ReadToEnd();
                File.WriteAllText(ConfigFilePath, content);
            }
        }

        public void OnAppUpdate(Version version)
        {

        }

        public void OnAppUninstall(Version version)
        {
            File.Delete(ConfigFilePath);
        }
        
    }
}