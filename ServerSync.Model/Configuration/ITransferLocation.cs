using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Configuration
{
    /// <summary>
    /// Interface for the definition of a transfer location
    /// </summary>
    public interface ITransferLocation
    {
        /// <summary>
        /// The transfer location's name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The transfer location's root path
        /// </summary>
        string RootPath { get; }

        /// <summary>
        /// If set, specifies the maximum size all files in the transfer location combined.
        /// Comsumers of the tranfer location definition must not write anything to the transfer location if it would exceced this limit
        /// </summary>
        ByteSize.ByteSize? MaximumSize { get; }

    }
}
