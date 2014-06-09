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


            FolderComparer comparer = new FolderComparer(config);
            var result = comparer.Run();

            using (var writer = new StreamWriter(File.Open(Path.Combine(config.LogDirectory, String.Format("Missing from {0}.txt", config.Left.Name)), FileMode.Create)))
            {
                foreach (var item in result.MissingLeft)
                {
                    writer.WriteLine(item);
                }
            }

            using (var writer = new StreamWriter(File.Open(Path.Combine(config.LogDirectory, String.Format("Missing from {0}.txt", config.Right.Name)), FileMode.Create)))
            {
                foreach (var item in result.MissingRight)
                {
                    writer.WriteLine(item);
                }
            }

            using (var writer = new StreamWriter(File.Open(Path.Combine(config.LogDirectory, "Conflicts.txt"), FileMode.Create)))
            {
                foreach (var item in result.Conflicts)
                {
                    writer.WriteLine(item);
                }
            }


            stopWatch.Stop();

            Console.WriteLine("Elapsed Time : " + stopWatch.Elapsed.ToString());

            return 0;
        }


    }

}
