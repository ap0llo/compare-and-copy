using Microscope;
using ServerSync.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSync.Core.Filters.Visitor
{
    class ExpressionEvaluationVisitor : IFilterExpressionVisitor<bool, FileItem>
    {
        readonly IFilterExpression m_RootExpression;

        public ExpressionEvaluationVisitor(IFilterExpression rootExpression)
        {
            if(rootExpression == null)
            {
                throw new ArgumentNullException("rootExpression");
            }

            this.m_RootExpression = rootExpression;
        }


        public bool IsMatch(FileItem fileItem)
        {
            return m_RootExpression.Accept(this, fileItem);
        }


        public bool Visit(AndFilterExpression expression, FileItem parameter)
        {
            return expression.Expressions.All(ex => ex.Accept(this, parameter));
        }

        public bool Visit(OrFilterExpression expression, FileItem parameter)
        {
            return expression.Expressions.Any(ex => ex.Accept(this, parameter));
        }

        public bool Visit(NotFilterExpression expression, FileItem parameter)
        {
            return !expression.NegatedExpression.Accept(this, parameter);
        }

        public bool Visit(RegexFilterExpression expression, FileItem parameter)
        {
            return expression.Regex.IsMatch(parameter.RelativePath);
        }

        public bool Visit(MicroscopeFilterExpression expression, FileItem parameter)
        {
            var evaluator = new QueryEvaluator(expression.Query);
            return evaluator.Evaluate(parameter.RelativePath);
        }

        public bool Visit(CompareStateFilterExpression expression, FileItem parameter)
        {
            return parameter.CompareState == expression.CompareState;
        }

        public bool Visit(TransferStateFilterExpression expression, FileItem parameter)
        {
            return parameter.TransferState == expression.TransferState;
        }
    }
}
