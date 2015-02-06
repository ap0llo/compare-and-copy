
using ServerSync.Core.PathResolving;
using System.Xml.Linq;

namespace ServerSync.Model.Configuration
{
    /// <summary>
    /// Interface for configuration readers
    /// </summary>
    public interface IConfigurationReader
    {
        /// <summary>
        /// Deserializes the specified sync configuration file
        /// </summary>
        ISyncConfiguration ReadConfiguration(string fileName);

        /// <summary>
        /// Deserializes the specified sync configuration file
        /// </summary>
        ISyncConfiguration ReadConfiguration(XDocument configuration, IPathResolver pathResolver);     
    }
}
