using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Model.Filtering
{
    /// <summary>
    /// Filter element that uses the Microscope framework (https://github.com/clotheshorse/microscope) to find matches.
    /// The Microscope query is applied to RelativePath property of <see cref="FileItem"/>
    /// </summary>
    public class MicroscopeFilterExpression : IFilterExpression
    {

        #region Fields

        readonly string m_Query;        

        #endregion


        #region Properties

        public string Query { get { return m_Query; } }

        #endregion


        #region Constructor

        public MicroscopeFilterExpression(string query)
        {
            this.m_Query = query;            
        }

        #endregion


        #region IFilterExpression Implementation

        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}
