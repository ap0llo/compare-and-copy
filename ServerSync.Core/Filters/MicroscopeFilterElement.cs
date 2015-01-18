using Microscope;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    /// <summary>
    /// Filter element that uses the Microscope framework (https://github.com/clotheshorse/microscope) to find matches.
    /// The Microscope query is applied to RelativePath property of <see cref="FileItem"/>
    /// </summary>
    class MicroscopeFilterElement : IFilterElement
    {

        #region Fields

        readonly string m_Query;
        readonly QueryEvaluator m_Evaluator;

        #endregion


        #region Properties

        public string Query
        {
            get { return m_Query; }
        }

        #endregion


        #region Constructor

        public MicroscopeFilterElement(string query)
        {
            this.m_Query = query;
            this.m_Evaluator = new QueryEvaluator(query);
        }

        #endregion


        #region IFilterElement Implementation

        public bool IsMatch(FileItem item)
        {
            return m_Evaluator.Evaluate(item.RelativePath);
        }

        #endregion

    }
}
