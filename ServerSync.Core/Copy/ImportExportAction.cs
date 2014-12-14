using ServerSync.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    /// <summary>
    /// Base class for both Import and Export action
    /// </summary>
    abstract class ImportExportAction : IOAction
    {

        #region Properties        

        public string TransferLocationName { get; set; }

        public string TransferLocationSubPath { get; set; }
       
        #endregion
        


    }
}