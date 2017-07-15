using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using CompareAndCopy.Core.Configuration;
using CompareAndCopy.Model.Configuration;
using CompareAndCopy.Model.State;

namespace CompareAndCopy.Core.State
{
    public class UpdateTransferStateAction : AbstractAction
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();
        

        public override string Name => "UpdateTransferStateAction";

        public IEnumerable<TransferLocationReference> TransferLocationPaths { get; }

        public IEnumerable<string> InterimLocations { get; }


        public UpdateTransferStateAction(bool isEnabled, ISyncConfiguration configuration, 
                                         string inputFilterName, 
                                         IEnumerable<TransferLocationReference> transferLocationPaths,
                                         IEnumerable<string> interimLocations)
            : base(isEnabled, configuration, inputFilterName)
        {
            TransferLocationPaths = transferLocationPaths?.ToList() ?? throw new ArgumentNullException(nameof(transferLocationPaths));
            InterimLocations = interimLocations?.ToList() ?? throw new ArgumentNullException(nameof(interimLocations));
        }



        public override void Run()
        {
            if (Flags.EnabledExtendedTransferState == false)
            {
                m_Logger.Info($"Extended TransferState is disabled, action {Name} has no effect");
                return;
            }

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
    }
}
