using System.Xml.Linq;

namespace CompareAndCopy.Core.Configuration.Migration
{
    public interface IConfigurationMigrator
    {
        XDocument UpgradeConfigurationFile(XDocument currentConfiguration);
    }
}
