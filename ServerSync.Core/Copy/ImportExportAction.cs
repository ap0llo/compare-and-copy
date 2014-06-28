using ServerSync.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Copy
{
    abstract class ImportExportAction : IOAction
    {

        #region Properties
        

        public string TransferLocation { get; set; }

        #endregion


    }
}