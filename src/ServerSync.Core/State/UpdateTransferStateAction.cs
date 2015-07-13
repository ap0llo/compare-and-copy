using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ServerSync.Core.Configuration;
using ServerSync.Model.Configuration;
using ServerSync.Model.State;

namespace ServerSync.Core.State
{
    public class UpdateTransferStateAction : AbstractAction
    {

        #region Fields

        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        #endregion


        #region Properties

        public IEnumerable<TransferLocationReference> TransferLocationPaths { get; private set; }

        public IEnumerable<string> InterimLocations { get; private set; }

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
                                         string inputFilterName, 
                                         IEnumerable<TransferLocationReference> transferLocationPaths,
                                         IEnumerable<string> interimLocations)
            : base(isEnabled, configuration, inputFilterName)
        {

            if(transferLocationPaths == null)
            {
                throw new ArgumentNullException("transferLocationPaths");
            }

            if(interimLocations == null)
            {
                throw new ArgumentNullException("interimLocations");
            }

            this.TransferLocationPaths = transferLocationPaths.ToList();
            this.InterimLocations = interimLocations.ToList();
        }
        #endregion


        #region Public Methods

        public override void Run()
        {

            var allPaths = TransferLocationPaths.Select(t => Path.Combine(Configuration.GetTransferLocation(t.TransferLocationName).RootPath, 
                                                                          t.TransferLocationSubPath))
                                                .Union(InterimLocations).ToList();

            var state = GetFilteredInput();

            //update the list of transfer locations the file exists in
            foreach(var file in state.Where(f=> f.TransferState.Direction != TransferDirection.None))
            {               
                foreach (var path in allPaths.Where(Directory.Exists))
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
            var filesToRemove = state.Where(file => !file.TransferState.Locations.Any()).ToList();
            foreach (var item in filesToRemove)
            {
                m_Logger.Info("Removing file '{0}' from sync state", item.RelativePath);
                State.RemoveFile(item);
            }
        }
        
        #endregion

    }
}
