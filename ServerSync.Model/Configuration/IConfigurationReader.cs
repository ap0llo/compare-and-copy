﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Configuration
{
    public interface IConfigurationReader
    {
        ISyncConfiguration ReadConfiguration(string fileName);       
    }
}
