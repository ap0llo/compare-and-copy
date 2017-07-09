using NLog;
using ServerSync.Core.State;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;
using System;

namespace ServerSync.Core
{
    public class JobRunner
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();
        readonly ISyncConfiguration m_SyncJob;

        
        public JobRunner(ISyncConfiguration syncJob)
        {
            m_SyncJob = syncJob ?? throw new ArgumentNullException(nameof(syncJob));
        }
        

        public bool Run()
        {
            ISyncState currentState = new SyncState();

            //execute all actions specified in the sync configuration
            foreach (var action in m_SyncJob.Actions)
            {
                if (!action.IsEnabled)
                {
                    continue;
                }


                //set configuration and state                    
                action.State = currentState;

                m_Logger.Info("Starting Action '{0}'", action.Name);

                try
                {
                    //run the action
                    action.Run();
                }
                catch (JobExecutionException ex)
                {
                    m_Logger.Error("Job did not run successfully: " + ex.Message);
                    return false;
                }

                //update the state
                currentState = action.State;
            }

            return true;
        }        
    }
}
