using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.State
{
    /// <summary>
    /// Definition of all values a comparison of files can yield
    /// </summary>
    public enum CompareState
    {
        /// <summary>
        /// The file is only present in the right sync folder
        /// </summary>
        MissingLeft,        

        /// <summary>
        /// The file is only present in the left folder
        /// </summary>
        MissingRight,       

        /// <summary>
        /// The file is present in both sync folders but in different versions
        /// </summary>
        Conflict
    }
}
