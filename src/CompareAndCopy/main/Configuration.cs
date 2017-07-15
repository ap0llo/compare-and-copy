using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using CompareAndCopy.Installation;

namespace CompareAndCopy
{
    sealed class Configuration
    {
        public const string ConfigFileName = "config.json";
        public const string DebugConfigFileName = "config.Debug.json";
        const string s_UpdateOptionsSectionName = "Update";
        const string s_FlagsOptionsSectionName = "Flags";


        public static Configuration Current { get; }


        static Configuration()
        {
            var root = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(ApplicationInfo.RootDirectory, ConfigFileName), true)
                .AddJsonFile(Path.Combine(ApplicationInfo.RootDirectory, DebugConfigFileName), true)
                .Build();

            var updateOptions = GetSection<UpdateOptions>(root, s_UpdateOptionsSectionName);
            var flagsOptions = GetSection<FlagsOptions>(root, s_FlagsOptionsSectionName);

            Current = new Configuration(updateOptions, flagsOptions);
        }

        
        static T GetSection<T>(IConfigurationRoot configurationRoot, string sectionName) where T : class, new()
        {
            var section = configurationRoot.GetSection(sectionName).Get<T>();
            section = section ?? new T();

            return section;
        }
        

        public UpdateOptions UpdateOptions { get; }

        public FlagsOptions FlagsOptions { get; }


        private Configuration([NotNull] UpdateOptions updateOptions, [NotNull] FlagsOptions flagsOptions)
        {
            FlagsOptions = flagsOptions ?? throw new ArgumentNullException(nameof(flagsOptions));
            UpdateOptions = updateOptions ?? throw new ArgumentNullException(nameof(updateOptions));
        }
    }
}