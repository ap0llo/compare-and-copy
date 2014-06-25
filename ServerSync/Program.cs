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

namespace ServerSync
{
    class Program
    {
        static int Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();


            if(args.Length != 1)
            {
                Console.WriteLine("You need to specify a Sync Configuration file");
                return 1;
            }

            var configFilePath = Path.GetFullPath(args[0]);

            if(!File.Exists(configFilePath))
            {
                Console.WriteLine("Could not find config file at '{0}'", configFilePath);
                return 2;
            }

            var config = new ConfigurationReader().ReadConfiguration(configFilePath);


            var currentState = new SyncState();

            if(File.Exists(GetSyncStateFilePath(config)))
            {
                var reader = new SyncStateStateReader();
                currentState = reader.ReadSyncState(GetSyncStateFilePath(config));
            }


            foreach(var action in config.Actions)
            {
                if(action.IsEnabled)
                {
                    action.Configuration = config;
                    action.State = currentState;

                    action.Run();

                    currentState = action.State;
                }
            }


            SyncStateWriter writer = new SyncStateWriter();
            writer.WriteSyncState(GetSyncStateFilePath(config), currentState);

            stopWatch.Stop();

            Console.WriteLine("Elapsed Time : " + stopWatch.Elapsed.ToString());

            return 0;
        }


        private static string GetSyncStateFilePath(SyncConfiguration config)
        {
            return Path.Combine(config.LogDirectory, "SyncState.xml");
        }




    }

}
