using NLog;
using ServerSync.Core;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using ServerSync.Model.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace ServerSync
{
    class Program
    {

        const string s_ServerSyncIniFileName = "ServerSync.ini";
        static readonly Logger s_Logger = LogManager.GetCurrentClassLogger();
        

        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            //Display Version Information
            WriteVersionInfo();            

            LoadFlags();

            //Check arguments
            if(args.Length < 1)
            {
                s_Logger.Error("You need to specify at least one Sync Configuration file");
                return 1;
            }

            var success = true;

            //Execute all configuration files specified via commandline
            foreach (var arg in args)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                //Load file
                var configFilePath = Path.GetFullPath(arg);
                if (!File.Exists(configFilePath))
                {
                    s_Logger.Error("Could not find configuration file at '{0}'", configFilePath);
                    return 2;
                }

                ISyncConfiguration config;
                try
                {
                    config = new ConfigurationReader().ReadConfiguration(configFilePath);
                }
                catch(ConfigurationException ex)
                {
                    s_Logger.Error(ex, "Error reading configuration");
                    return 1;
                }


                var jobRunner = new JobRunner(config);
                success &= jobRunner.Run();
                
                stopWatch.Stop();

                s_Logger.Info("Elapsed Time : " + stopWatch.Elapsed.ToString());
            }
           
            return success ? 0 : 1;
        }

        
        /// <summary>
        /// Writes Application Name and Version to the Console
        /// </summary>
        static void WriteVersionInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();            
            s_Logger.Info("{0}, Version {1}", assembly.GetName().Name, assembly.GetName().Version);
        }

        static void LoadFlags()
        {
            var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileName = Path.Combine(directory, s_ServerSyncIniFileName);

            if (File.Exists(fileName))
            {                
                var config = new ConfigurationBuilder()
                    .AddIniFile(fileName)
                    .Build();

                Flags.EnabledExtendedTransferState = GetFlag(config, nameof(Flags.EnabledExtendedTransferState));                
            }
        }

        static bool GetFlag(IConfiguration configuration, string flagName)
        {
            var strValue = configuration[$"Flags:{flagName}"];
            return strValue != null && bool.Parse(strValue);
        }

        static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e?.ExceptionObject is Exception ex)
            {
                s_Logger.Fatal(ex, "Unhandled exception");
            }
            else
            {
                s_Logger.Fatal("Unknown unhandled exception");
            }
            Environment.Exit(2);
        }
    }
}
