using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Contrib.Linq
{
    internal class InnermostWhereFinder : ExpressionVisitor
    {
        private MethodCallExpression _innermostWhereExpression;

        public MethodCallExpression GetInnermostWhere(Expression expression)
        {
            Visit(expression);
            return _innermostWhereExpression;
        }



        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (expression.Method.Name == "All" ||
                expression.Method.Name == "Any" ||
                expression.Method.Name == "First" ||
                expression.Method.Name == "FirstOrDefault" ||
                expression.Method.Name == "Last" ||
                expression.Method.Name == "LastOrDefault" ||
                expression.Method.Name == "Select" ||
                expression.Method.Name == "Single" ||
                expression.Method.Name == "SingleOrDefault" ||
                expression.Method.Name == "Where" )
                _innermostWhereExpression = expression;

            Visit(expression.Arguments[0]);

            return expression;
        }
    }
}
