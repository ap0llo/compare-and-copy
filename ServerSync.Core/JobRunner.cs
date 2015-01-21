using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core
{
    public class JobRunner
    {

        #region Fields

        Logger m_Logger = LogManager.GetCurrentClassLogger();
        ISyncConfiguration m_SyncJob;

        #endregion


        #region Constructor

        public JobRunner(ISyncConfiguration syncJob)
        {
            if(syncJob == null)
            {
                throw new ArgumentNullException("syncJob");
            }

            this.m_SyncJob = syncJob;
        }


        #endregion


        #region Public Methods
        
        public bool Run()
        {
            ISyncState currentState = new SyncState();

            //execute all actions specified in the sync configuration
            foreach (var action in m_SyncJob.Actions)
            {
                if (action.IsEnabled)
                {
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
            }

            return true;
        }        
        
        #endregion

    }
}
