using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

        internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, Dictionary<string, string> propertyList)
        {
            if (propertyList == null) return false;
            return ((exp is MemberExpression) &&
                    (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
                    propertyList[(((MemberExpression)exp).Member.Name)] != null);
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

        internal static string GetIndentifierFromExpression(Expression expression)
        {
            return GetTableFromExpression(expression).Identifier;
        }

        internal static TableHelper GetTableFromExpression(Expression expression)
        {
            var exp = GetMemberExpression(expression);
            if (!(exp is MemberExpression)) return null;

            return CacheHelper.TryGetTable(((MemberExpression)exp).Expression.Type);
        }

        internal static string GetPropertyNameWithIdentifierFromExpression(Expression expression)
        {
            var exp = GetMemberExpression(expression);
            if (!(exp is MemberExpression)) return string.Empty;

            var table = CacheHelper.TryGetTable(((MemberExpression)exp).Expression.Type);
            var member = ((MemberExpression)exp).Member;

            return string.Format("{0}.[{1}]", table.Identifier, table.Columns[member.Name]);
        }

        internal static string GetPropertyNameFromExpression(Expression expression)
        {
            var exp = GetMemberExpression(expression);
            if (!(exp is MemberExpression)) return string.Empty;

            var member = ((MemberExpression)exp).Member;
            var columns = CacheHelper.TryGetPropertyList(((MemberExpression)exp).Expression.Type);
            return columns[member.Name];
        }

        internal static Expression GetMemberExpression(Expression expression)
        {
            if (expression is UnaryExpression)
                return GetMemberExpression((((UnaryExpression)expression).Operand));
            if (expression is LambdaExpression)
                return GetMemberExpression((((LambdaExpression)expression).Body));
            if (expression is MemberExpression)
                return expression;
            return null;
        }

        internal static object GetValueFromExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        internal static string GetOperator(string methodName)
        {
            switch (methodName)
            {
                case "Add": return "+";
                case "Subtract": return "-";
                case "Multiply": return "*";
                case "Divide": return "/";
                case "Negate": return "-";
                case "Remainder": return "%";
                default: return null;
            }
        }

        internal static string GetOperator(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    return "-";
                case ExpressionType.UnaryPlus:
                    return "+";
                case ExpressionType.Not:
                    return IsBoolean(u.Operand.Type) ? "NOT" : "~";
                default:
                    return "";
            }
        }

        internal static string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (IsBoolean(b.Left.Type)) ? "AND" : "&";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (IsBoolean(b.Left.Type) ? "OR" : "|");
                default:
                    return GetOperator(b.NodeType);
            }
        }

        internal static string GetOperator(ExpressionType exprType)
        {
            switch (exprType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.RightShift:
                    return ">>";
                default:
                    return "";
            }
        }

        internal static bool IsHasValue(Expression expr)
        {
            return (expr is MemberExpression) && (((MemberExpression)expr).Member.Name == "HasValue");
        }

        internal static bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        internal static bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsBoolean(expr.Type);
                case ExpressionType.Not:
                    return IsBoolean(expr.Type);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return true;
                case ExpressionType.Call:
                    return IsBoolean(expr.Type);
                default:
                    return false;
            }
        }

        internal static bool IsVariable(Expression expr)
        {
            return (expr is MemberExpression) && (((MemberExpression)expr).Expression is ConstantExpression);
        }

        internal static TableHelper GetTypeProperties(Type type)
        {
            var table = CacheHelper.TryGetTable(type);
            if (table.Name != null) return table; // have table in cache

            // get properties add to cache
            var properties = new Dictionary<string, string>();
            type.GetProperties().ToList().ForEach(
                    x =>
                    {
                        var col = (ColumnAttribute)x.GetCustomAttribute(typeof(ColumnAttribute));
                        properties.Add(x.Name, (col != null) ? col.Name : x.Name);
                    }
                );


            var attrib = (TableAttribute)type.GetCustomAttribute(typeof(TableAttribute));

            table = new TableHelper
            {
                Name = (attrib != null ? attrib.Name : type.Name),
                Columns = properties,
                Identifier = string.Format("t{0}", CacheHelper.Size + 1)
            };
            CacheHelper.TryAddTable(type, table);

            return table;
        }
    }
}
