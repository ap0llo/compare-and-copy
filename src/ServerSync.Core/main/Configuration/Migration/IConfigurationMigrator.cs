using System.Xml.Linq;

namespace ServerSync.Core.Configuration.Migration
{
    public interface IConfigurationMigrator
    {
        XDocument UpgradeConfigurationFile(XDocument currentConfiguration);
    }
}
