using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSync.Core;
using ServerSync.Core.Compare;
using ServerSync.Core.Configuration;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ServerSync.Core.State;
using System.Reflection;

namespace ServerSync
{
    class Program
    {
        public static int Main(string[] args)
        {
            WriteVersionInfo();            

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if(args.Length < 1)
            {
                Console.WriteLine("You need to specify a Sync Configuration file");
                return 1;
            }


            for(int i = 0; i < args.Length; i++)
            {
                var configFilePath = Path.GetFullPath(args[i]);

                if (!File.Exists(configFilePath))
                {
                    Console.WriteLine("Could not find config file at '{0}'", configFilePath);
                    return 2;
                }

                var config = new ConfigurationReader().ReadConfiguration(configFilePath);
                ExecuteJob(config);

                stopWatch.Stop();

                Console.WriteLine("Elapsed Time : " + stopWatch.Elapsed.ToString());
            }
           
            return 0;
        }


        private static void WriteVersionInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine("{0}, Version {1}", assembly.GetName().Name, assembly.GetName().Version);
            Console.WriteLine();
        }

        private static void ExecuteJob(SyncConfiguration configuration)
        {
            var currentState = new SyncState();

            foreach (var action in configuration.Actions)
            {
                if (action.IsEnabled)
                {
                    action.Configuration = configuration;
                    action.State = currentState;

                    Console.WriteLine("Starting Action '{0}'", action.Name);

                    action.Run();

                    currentState = action.State;
                }
            }            
        }

    }

}
