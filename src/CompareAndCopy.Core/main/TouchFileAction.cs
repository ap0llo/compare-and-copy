using CompareAndCopy.Model.Configuration;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompareAndCopy.Core
{
    public class TouchFileAction : AbstractAction
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();


        public override string Name => "TouchFile";

        public string Path { get; }


        public TouchFileAction(bool isEnabled, ISyncConfiguration configuration, string path) 
            : base(isEnabled, configuration, null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value must not be empty", nameof(path));

            Path = path;
        }


        public override void Run()
        {
            if (!File.Exists(Path))
            {
                m_Logger.Debug($"Creating empty file '{Path}'");
                using (File.Open(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                { }
            }

            m_Logger.Debug($"Setting last write time for file '{Path}'");
            File.SetLastWriteTimeUtc(Path, DateTime.UtcNow);
        }
    }
}
