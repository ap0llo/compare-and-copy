﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Configuration
{
    public interface ISyncFolderDefinition
    {

        string Name { get; }

        string RootPath { get; }

    }
}
