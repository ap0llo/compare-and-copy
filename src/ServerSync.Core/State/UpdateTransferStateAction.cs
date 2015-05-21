using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;

namespace ServerSync.Core.State
{
    public class UpdateTransferStateAction : AbstractAction
    {

        #region Properties


        public IEnumerable<string> TranferLocations { get; private set; }

        public override string Name
        {
            get
            {
                return "UpdateTransferStateAction";
            }
        }

        #endregion


        #region Constructor


        public UpdateTransferStateAction(bool isEnabled, ISyncConfiguration configuration, 
                                         string inputFilterName, SyncFolder syncFolder,
                                         IEnumerable<string> transferLocations)
            : base(isEnabled, configuration, inputFilterName)
        {

            if(transferLocations == null)
            {
                throw new ArgumentNullException("transferLocations");
            }

            this.TranferLocations = transferLocations.ToList();
        }
        #endregion


        #region Public Methods

        public override void Run()
        {
            var state = GetFilteredInput();

            //update the list of transfer locations the file exists in
            foreach(var file in state)
            {
                if(file.TransferState.Direction == TransferDirection.None)
                {
                    continue;
                }

                foreach (var path in TranferLocations)
                {
                    var absolutePath = Path.Combine(path, file.RelativePath);
                    if(File.Exists(absolutePath))
                    {
                        file.TransferState.AddTransferLocation(path);
                    } 
                    else
                    {
                        file.TransferState.RemoveTransferLocation(path);
                    }
                }
            }

            //remove all files from sync state that are not present in any transfer location
            var filesToRemove = state.Where(file => !file.TransferState.TranferLocations.Any()).ToList();
            foreach (var item in filesToRemove)
            {
                State.RemoveFile(item);
            }
        }
        
        #endregion

    }
}
