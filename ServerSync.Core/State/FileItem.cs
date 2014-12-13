﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.State
{
    public class FileItem
    {
        public string RelativePath { get; set; }

        public CompareState CompareState { get; set; }

        public TransferState TransferState { get; set; }
    }
}