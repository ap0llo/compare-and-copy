using NLog;
using ServerSync.Core;
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

        #region Fields


        static Logger s_Logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Main

        public static int Main(string[] args)
        {
            //Display Version Information
            WriteVersionInfo();            

            //Check arguments
            if(args.Length < 1)
            {
                s_Logger.Error("You need to specify at least one Sync Configuration file");
                return 1;
            }

            bool success = true;

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


                var jobRunner = new JobRunner(config);
                success &= jobRunner.Run();
                
                stopWatch.Stop();

                s_Logger.Info("Elapsed Time : " + stopWatch.Elapsed.ToString());
            }
           
            return success ? 0 : 1;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Writes Application Name and Version to the Console
        /// </summary>
        static void WriteVersionInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();            
            s_Logger.Info("{0}, Version {1}", assembly.GetName().Name, assembly.GetName().Version);
        }

        #endregion

    }

}
