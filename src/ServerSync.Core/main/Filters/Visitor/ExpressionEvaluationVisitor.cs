using Microscope;
using ServerSync.Core.State;
using ServerSync.Model;
using ServerSync.Model.Filtering;
using ServerSync.Model.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters
{
    class ExpressionEvaluationVisitor : IFilterExpressionVisitor<bool, IFileItem>
    {

        #region Fields

        readonly IFilterExpression m_RootExpression;

        #endregion


        #region Constructor

        public ExpressionEvaluationVisitor(IFilterExpression rootExpression)
        {
            if(rootExpression == null)
            {
                throw new ArgumentNullException("rootExpression");
            }

            this.m_RootExpression = rootExpression;
        }

        #endregion


        #region Public Methods

        public bool IsMatch(IFileItem fileItem)
        {
            return m_RootExpression.Accept(this, fileItem);
        }

        #endregion


        #region IFilterExpressionVisitor Implementation

        public bool Visit(AndFilterExpression expression, IFileItem parameter)
        {
            return expression.Expressions.All(ex => ex.Accept(this, parameter));
        }

        public bool Visit(OrFilterExpression expression, IFileItem parameter)
        {
            return expression.Expressions.Any(ex => ex.Accept(this, parameter));
        }

        public bool Visit(NotFilterExpression expression, IFileItem parameter)
        {
            return !expression.NegatedExpression.Accept(this, parameter);
        }

        public bool Visit(RegexFilterExpression expression, IFileItem parameter)
        {
            return expression.Regex.IsMatch(parameter.RelativePath);
        }

        public bool Visit(MicroscopeFilterExpression expression, IFileItem parameter)
        {
            var evaluator = new QueryEvaluator(expression.Query);
            return evaluator.Evaluate(parameter.RelativePath);
        }

        public bool Visit(CompareStateFilterExpression expression, IFileItem parameter)
        {
            return parameter.CompareState == expression.CompareState;
        }

        public bool Visit(TransferStateFilterExpression expression, IFileItem parameter)
        {
            return parameter.TransferState.Direction == expression.TransferState;
        }

        #endregion

    }
}
