using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dapper.Contrib.Linq2Dapper.Helpers
{
    internal class Helper
    {
        internal static bool IsEqualsExpression(Expression exp)
        {
            return exp.NodeType == ExpressionType.Equal || exp.NodeType == ExpressionType.NotEqual;
        }

        internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName)
        {
            return ((exp is MemberExpression) &&
                (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                (((MemberExpression)exp).Member.Name == memberName));
        }

        internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, List<string> propertyList)
        {
            return ((exp is MemberExpression) &&
                (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                propertyList.Contains((((MemberExpression)exp).Member.Name)));
        }

        internal static object GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType)
        {
            if (!IsEqualsExpression(be))
                throw new Exception("There is a bug in this program.");

            if (be.Left.NodeType == ExpressionType.MemberAccess)
            {
                var me = (MemberExpression)be.Left;

                if (me.Member.DeclaringType == memberDeclaringType)
                {
                    return GetValueFromExpression(be.Right);
                }
            }
            else if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                var me = (MemberExpression)be.Right;

                if (me.Member.DeclaringType == memberDeclaringType)
                {
                    return GetValueFromExpression(be.Left);
                }
            }

            // We should have returned by now. 
            throw new Exception("There is a bug in this program.");
        }

        internal static string GetPropertyNameFromEqualsExpression(BinaryExpression be, Type memberDeclaringType)
        {
            if (!IsEqualsExpression(be))
                throw new Exception("There is a bug in this program.");

            if (be.Left.NodeType == ExpressionType.MemberAccess)
            {
                return GetPropertyNameFromExpression(be.Left);
            }
            if (be.Right.NodeType == ExpressionType.MemberAccess)
            {
                return GetPropertyNameFromExpression(be.Right);
            }

            // We should have returned by now. 
            throw new Exception("There is a bug in this program.");
        }

        internal static string GetPropertyNameFromExpression(Expression expression)
        {
            if (expression is MemberExpression)
                return (((MemberExpression)expression).Member.Name);
            if (expression is UnaryExpression)
                return GetPropertyNameFromExpression((((UnaryExpression)expression).Operand));
            if (expression is LambdaExpression)
                return GetPropertyNameFromExpression((((LambdaExpression)expression).Body));
            return string.Empty;
        }

        internal static object GetValueFromExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }
    }
}
