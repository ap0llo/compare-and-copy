using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    public class NotFilterExpression : IFilterExpression
    {

        #region Fields

        readonly IFilterExpression m_NegatedExpression;

        #endregion


        #region Properties

        public IFilterExpression NegatedExpression { get { return m_NegatedExpression; } }

        #endregion


        #region Constructor

        public NotFilterExpression(IFilterExpression negatedExpression)
        {
            if(negatedExpression == null)
            {
                throw new ArgumentNullException("negatedExpression");
            }

            this.m_NegatedExpression = negatedExpression;
        }

        #endregion


        #region IFilterExpression Implementation

        public bool IsMatch(FileItem item)
        {
            return ! NegatedExpression.IsMatch(item);
        }

        public T1 Accept<T1, T2>(IFilterExpressionVisitor<T1, T2> visitor, T2 parameter)
        {
            return visitor.Visit(this, parameter);
        }

        #endregion

    }
}