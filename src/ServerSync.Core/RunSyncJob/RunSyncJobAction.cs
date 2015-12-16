using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
	public class RunSyncJobAction : AbstractAction
	{

		#region Fields

		readonly Logger m_Logger = LogManager.GetCurrentClassLogger();
		readonly string m_ConfigurationPath;

		#endregion


		#region Properties

		public override string Name { get { return "RunSyncJob"; } }

		/// <summary>
		/// The path of the sync configuration file to load and execute
		/// </summary>
		public string ConfigurationPath { get { return m_ConfigurationPath; } }

		#endregion


		#region Constructor

		/// <summary>
		/// Initializes a new instance of RunSyncJobAction
		/// </summary>
		/// <param name="isEnabled"></param>
		/// <param name="configuration"></param>
		/// <param name="inputFilterName"></param>
		/// <param name="configurationPath">The path of the sync configuration to load and execute</param>
		/// <exception cref="ArgumentNullException">Thrown if configurationPath is null</exception>
		/// <exception cref="ArgumentException">Thrown if configurationPath is empty</exception>
		public RunSyncJobAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName, string configurationPath)
			: base(isEnabled, configuration, inputFilterName)
		{
			if (configurationPath == null)
				throw new ArgumentNullException("configurationPath");

			if (string.IsNullOrWhiteSpace(configurationPath))
				throw new ArgumentException("Value must not be empty", "configurationPath");

			this.m_ConfigurationPath = configurationPath;
		}

		#endregion

		public override void Run()
		{

			m_Logger.Info("Loading sync configuration from '{0}'", this.ConfigurationPath);
			var configurationReader = new ConfigurationReader();
			var configuration = configurationReader.ReadConfiguration(this.ConfigurationPath);

			m_Logger.Info("Executing sync job");

			var jobRunner = new JobRunner(configuration);
			var success = jobRunner.Run();

			m_Logger.Info("Sync job completed {0}", success ? "successfully" : "with errors");
		
		}
	}
}
