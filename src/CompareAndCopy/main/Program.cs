using NLog;
using CompareAndCopy.Core;
using CompareAndCopy.Core.Configuration;
using CompareAndCopy.Model.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CompareAndCopy.Installation;

namespace CompareAndCopy
{
    class Program
    {        
        static readonly Logger s_Logger = LogManager.GetCurrentClassLogger();
        

        public static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            // handle installation events in case the application was launched by squirrel after installation
            Installer.HandleInstallationEvents();

            //Display Version Information
            WriteVersionInfo();


            // initialize updater                        
            var updater = new Updater(Configuration.Current.UpdateOptions);
            updater.Start();

            LoadFlags();

            var exitCode = 0; 

            //Check arguments
            if (args.Length < 1)
            {
                s_Logger.Error("You need to specify at least one Sync Configuration file");
                exitCode = 1;
            }
            else
            {
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
                        exitCode = 2;
                        break;
                    }

                    ISyncConfiguration config;
                    try
                    {
                        config = new ConfigurationReader().ReadConfiguration(configFilePath);
                    }
                    catch(ConfigurationException ex)
                    {
                        s_Logger.Error(ex, "Error reading configuration");
                        exitCode = 1;
                        break;
                    }


                    var jobRunner = new JobRunner(config);
                    var success = jobRunner.Run();
                    if (!success)
                    {
                        exitCode = 1;
                    }
                
                    stopWatch.Stop();

                    s_Logger.Info("Elapsed Time : " + stopWatch.Elapsed.ToString());
                }
                
            }
            
            // wait for completion of updater
            if (updater.Status == UpdaterStatus.Running)
            {
                s_Logger.Info("Application update is in progress, awaiting completion");
            }
            updater.Stop();
            if (updater.Status == UpdaterStatus.Failed)
            {
                s_Logger.Warn($"Update failed: \n\t{updater.Error.Replace("\n", "\n\t")}");                
            }

            return exitCode;
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
            Flags.EnabledExtendedTransferState = Configuration.Current.FlagsOptions.EnabledExtendedTransferState;
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
