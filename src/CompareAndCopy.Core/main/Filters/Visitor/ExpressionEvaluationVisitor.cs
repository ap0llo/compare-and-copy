using Microscope;
using CompareAndCopy.Model.Filtering;
using CompareAndCopy.Model.State;
using System;
using System.Linq;

namespace CompareAndCopy.Core.Filters
{
    class ExpressionEvaluationVisitor : IFilterExpressionVisitor<bool, IFileItem>
    {
        readonly IFilterExpression m_RootExpression;


        public ExpressionEvaluationVisitor(IFilterExpression rootExpression)
        {
            m_RootExpression = rootExpression ?? throw new ArgumentNullException(nameof(rootExpression));
        }


        public bool IsMatch(IFileItem fileItem) => m_RootExpression.Accept(this, fileItem);


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
    }
}
