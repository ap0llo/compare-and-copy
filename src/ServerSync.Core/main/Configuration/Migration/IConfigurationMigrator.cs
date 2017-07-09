using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerSync.Core.Configuration.Migration
{
    public interface IConfigurationMigrator
    {

        XDocument UpgradeConfigurationFile(XDocument currentConfiguration);

    }
}
