using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ServerSync
{
    class Program
    {

        static Logger s_Logger = LogManager.GetCurrentClassLogger();


        public static int Main(string[] args)
        {
            //Display Version Information
            WriteVersionInfo();            


            //Check arguments
            if(args.Length < 1)
            {
                s_Logger.Error("You need to specify a Sync Configuration file");
                return 1;
            }

            //Execute all configuration files specified via commandline
            for(int i = 0; i < args.Length; i++)
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                //Load file
                var configFilePath = Path.GetFullPath(args[i]);
                if (!File.Exists(configFilePath))
                {
                    s_Logger.Error("Could not find configuration file at '{0}'", configFilePath);
                    return 2;
                }
                SyncConfiguration config = null;
                try
                {
                    config = new ConfigurationReader().ReadConfiguration(configFilePath);
                }
                catch(ConfigurationException ex)
                {
                    s_Logger.Error("Error reading configuration: " + ex.Message);
                    return 1;
                }

                //execute the jib
                ExecuteJob(config);

                stopWatch.Stop();

                s_Logger.Info("Elapsed Time : " + stopWatch.Elapsed.ToString());
            }
           
            return 0;
        }

        /// <summary>
        /// Writes Application Name and Version to the Console
        /// </summary>
        private static void WriteVersionInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();            
            s_Logger.Info("{0}, Version {1}", assembly.GetName().Name, assembly.GetName().Version);            
        }

        /// <summary>
        /// Executes the specified SyncConfiguration
        /// </summary>
        private static void ExecuteJob(SyncConfiguration configuration)
        {
            var currentState = new SyncState();

            //execute all actions specified in the sync configuration
            foreach (var action in configuration.Actions)
            {
                if (action.IsEnabled)
                {
                    //set configuration and state
                    action.Configuration = configuration;
                    action.State = currentState;

                    s_Logger.Info("Starting Action '{0}'", action.Name);

                    //run the action
                    action.Run();

                    //update the state
                    currentState = action.State;
                }
            }            
        }

    }

}
