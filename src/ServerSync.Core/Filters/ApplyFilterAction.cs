﻿using ServerSync.Core.Configuration;
using ServerSync.Core.State;
using ServerSync.Model.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class ApplyFilterAction : AbstractAction
    {

        #region Properties

        public override string Name
        {
            get { return "ApplyFilter"; }
        }
        

        #endregion Properties


        #region Constructor
        
        public ApplyFilterAction(bool isEnabled, ISyncConfiguration configuration, string inputFilterName)
            : base(isEnabled, configuration, inputFilterName)
        {
        }

        #endregion

        #region Public Methods

        public override void Run()
        { 
            this.State = new SyncState(GetFilteredInput());            
        }

        #endregion Public Methods

    }
}