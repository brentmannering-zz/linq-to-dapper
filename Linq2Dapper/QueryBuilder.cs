using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper.Contrib.Linq2Dapper.Exceptions;
using Dapper.Contrib.Linq2Dapper.Helpers;

namespace Dapper.Contrib.Linq2Dapper
{
    internal class QueryBuilder<TData> : ExpressionVisitor
    {
        #region Fields
        
        private int _nextParameter;
        private bool _notOperater;
        private StringBuilder _selectStatement;
        private readonly StringBuilder _whereClause;
        private readonly StringBuilder _orderBy;
        private int _topCount;
        private bool _isDistinct;
        private string _parameter
        {
            get { return string.Format("p{0}", _nextParameter += 1); }
        }

        #endregion

        #region Properties
        public string Sql
        {
            get
            {
                SelectStatement();
                return _selectStatement.ToString();
            }
        }
                
        public DynamicParameters Parameters { get; private set; }

        #endregion

        #region ctor

        public QueryBuilder()
        {
            Parameters = new DynamicParameters();
            _whereClause = new StringBuilder();
            _orderBy = new StringBuilder();
            

            
            Init();
        }

        #endregion

        #region Write
        protected void Write(object value)
        {
            _whereClause.Append(value);
        }

        protected virtual void WriteParameter(object val)
        {
            if (val == null) return;

            var param = _parameter;
            Parameters.Add(param, val);

            Write("@" + param);
        }

        protected virtual void WriteAliasName(string aliasName)
        {
            Write(aliasName);
        }

        protected virtual void WriteColumnName(string columnName)
        {
            Write("[" + columnName + "]");
        }

        private void SelectStatement()
        {
            var tableName = CacheHelper.TryGetTableName<TData>();
            var propList = CacheHelper.TryGetPropertyList<TData>();

            _selectStatement = new StringBuilder();
            
            _selectStatement.Append("SELECT ");

            if (_topCount > 0)
                _selectStatement.Append("TOP(" + _topCount + ") ");

            if (_isDistinct)
                _selectStatement.Append("DISTINCT ");
            
            for (int i = 0; i < propList.Count; i++)
            {
                var x = propList[i];
                _selectStatement.Append("[" + x + "]");

                if ((i + 1) != propList.Count)
                    _selectStatement.Append(",");

                _selectStatement.Append(" ");
            }

            _selectStatement.Append("FROM [" + tableName + "]");
            _selectStatement.Append(WriteClause());
        }

        private string WriteClause()
        {
            var clause = string.Empty;

            // WHERE
            if (!string.IsNullOrEmpty(_whereClause.ToString()))
                clause += " WHERE " + _whereClause;
            
            //ORDER BY
            if (!string.IsNullOrEmpty(_orderBy.ToString()))
                clause += " ORDER BY " + _orderBy;

            return  clause;
        }

        private void WriteIsNull()
        {
            Write(" IS");
            if (!_notOperater)
                Write(" NOT");
            Write(" NULL");
            _notOperater = false;
        }

        private void WriteLike()
        {
            if (_notOperater)
                Write(" NOT");
            Write(" LIKE ");
            _notOperater = false;
        }

        private void WriteIn()
        {
            if (_notOperater)
                Write(" NOT");
            Write(" IN ");
            _notOperater = false;
        }

        private void WriteOperator()
        {
            Write(GetOperator((_notOperater) ? ExpressionType.NotEqual : ExpressionType.Equal));
            _notOperater = false;
        }

        private void WriteOrder(string name, bool descending)
        {
            var order = new StringBuilder();
            order.Append("[" + name + "]");
            if (descending) order.Append(" DESC");
            if (!string.IsNullOrEmpty(_orderBy.ToString())) order.Append(", ");
            _orderBy.Insert(0, order);
        }


        #endregion

        #region Visitors

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.UnaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
                _notOperater = true;

            if (!(node.Operand is MemberExpression)) return base.VisitUnary(node);

            Visit(node.Operand);
            if (IsBoolean(node.Operand.Type) && !IsHasValue(node.Operand))
                Write((!IsPredicate(node) ? " <> " : " = ") + "0");
                
            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MemberExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitMember(MemberExpression node)
        {
            if (Helper.IsSpecificMemberExpression(node, typeof(TData), CacheHelper.TryGetPropertyList<TData>()))
            {
                WriteColumnName(Helper.GetPropertyNameFromExpression(node));
            }
            else if (IsVariable(node))
            {
                WriteParameter(Helper.GetValueFromExpression(node));
                return node;
            }
            else if (IsHasValue(node))
            {
                var me = base.VisitMember(node);
                WriteIsNull();
                return me;
            }
            return base.VisitMember(node); ;
        }

        /// <summary>
        /// Visits the <see cref="T:System.Linq.Expressions.ConstantExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            object val;
            if (node.Type == typeof(Linq2Dapper<TData>)) return node;
            if (node.Value is ConstantExpression)
                val = Helper.GetValueFromExpression((ConstantExpression)node.Value);
            else
                val = Helper.GetValueFromExpression(node);

            WriteParameter(val);

            return base.VisitConstant(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.BinaryExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            var op = GetOperator(node);
            Expression left = node.Left;
            Expression right = node.Right;

            Write("(");

            if (IsBoolean(left.Type))
            {
                Visit(left);
                Write(" ");
                Write(op);
                Write(" ");
                Visit(right);
            }
            else
            {
                VisitValue(left);
                Write(" ");
                Write(op);
                Write(" ");
                VisitValue(right);
            }

            Write(")");

            return node;
        }

        /// <summary>
        /// Visits the children of the <see cref="T:System.Linq.Expressions.MethodCallExpression"/>.
        /// </summary>
        /// <returns>
        /// The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.
        /// </returns>
        /// <param name="node">The expression to visit.</param>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "EndsWith":
                case "StartsWith":
                case "Contains":
                    return LikeInMethod(node);
                case "IsNullOrEmpty":
                    // ISNULL(...)
                    if (Helper.IsSpecificMemberExpression(node.Arguments[0], typeof(TData), CacheHelper.TryGetPropertyList<TData>()))
                    {
                        Write("ISNULL(");
                        Visit(node.Arguments[0]);
                        Write(", '') ");
                        WriteOperator();
                        Write(" ''");
                        return node;
                    }
                    break;
                case "Join":
                    throw new InvalidQueryException("Invalid join query");
                case "Take":
                    // TOP(..)
                    _topCount = (int)Helper.GetValueFromExpression(node.Arguments[1]);
                    return node;
                case "Single":
                case "First":
                case "FirstOrDefault":
                    // TOP(1)
                    _topCount = 1;
                    return Visit(node.Arguments[1]);
                case "Distinct":
                    // DISTINCT
                    _isDistinct = true;
                    return node;
                case "OrderBy":
                case "ThenBy":
                case "OrderByDescending":
                case "ThenByDescending":
                    // ORDER BY ...
                    WriteOrder(Helper.GetPropertyNameFromExpression(node.Arguments[1]), node.Method.Name.Contains("Descending"));
                    return Visit(node.Arguments[0]);
            }
            return base.VisitMethodCall(node);
        }

        protected virtual Expression VisitValue(Expression expr)
        {
            return Visit(expr);
        }

        protected virtual Expression VisitPredicate(Expression expr)
        {
            if (!IsPredicate(expr) && !IsHasValue(expr))
            {
                Write(" <> 0");
            }
            return expr;
        }

        #endregion

        #region Helpers

        private Expression LikeInMethod(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(string))
            {
                // LIKE '..'
                if (!Helper.IsSpecificMemberExpression(node.Object, typeof(TData), CacheHelper.TryGetPropertyList<TData>()))
                    return node;

                Visit(node.Object);
                WriteLike();
                if (node.Method.Name == "EndsWith" || node.Method.Name == "Contains") Write("'%' + ");
                Visit(node.Arguments[0]);
                if (node.Method.Name == "StartsWith" || node.Method.Name == "Contains") Write("+ '%'");
                return node;
            }

            // IN (...)
            object ev = null;

            if (node.Method.DeclaringType == typeof (List<string>))
            {
                if (
                    !Helper.IsSpecificMemberExpression(node.Arguments[0], typeof (TData),
                        CacheHelper.TryGetPropertyList<TData>()))
                    return node;


                Visit(node.Arguments[0]);
                ev = Helper.GetValueFromExpression(node.Object);

            }
            else if (node.Method.DeclaringType == typeof (Enumerable))
            {
                if (
                    !Helper.IsSpecificMemberExpression(node.Arguments[1], typeof (TData),
                        CacheHelper.TryGetPropertyList<TData>()))
                    return node;

                Visit(node.Arguments[1]);
                ev = Helper.GetValueFromExpression(node.Arguments[0]);

            }
            else
            {
                return node;
            }

            WriteIn();

            // Add each string in the collection to the list of locations to obtain data about. 
            var queryStrings = (IEnumerable<object>)ev;
            var count = queryStrings.Count();
            Write("(");
            for (int i = 0; i < count; i++)
            {
                WriteParameter(queryStrings.ElementAt(i));

                if (i + 1 < count)
                    Write(", ");
            }
            Write(")");

            return node;
        }



        protected virtual string GetOperator(string methodName)
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

        protected virtual string GetOperator(UnaryExpression u)
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

        protected virtual string GetOperator(BinaryExpression b)
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

        protected virtual string GetOperator(ExpressionType exprType)
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

        protected virtual bool IsHasValue(Expression expr)
        {
            return (expr is MemberExpression) && (((MemberExpression)expr).Member.Name == "HasValue");
        }

        protected virtual bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

        protected virtual bool IsPredicate(Expression expr)
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

        protected virtual bool IsVariable(Expression expr)
        {
            return (expr is MemberExpression) && (((MemberExpression)expr).Expression is ConstantExpression);
        }

        #endregion


        private void Init()
        {
            GetTypeProperties();
        }

        private void GetTypeProperties()
        {
            var queryType = typeof(TData);
            

            try
            {
                CacheHelper.TryGetPropertyList<TData>();
                return;
            }
            catch (Exception ex) { }
             
            
            var properties = new List<string>();
            properties.AddRange(
                queryType.GetProperties().Select(x =>
                {
                    var col = (ColumnAttribute) x.GetCustomAttribute(typeof (ColumnAttribute));
                    return (col != null) ? col.Name : x.Name;
                }).ToList()
            );
            CacheHelper.TryAddPropertyList<TData>(properties);

            var attrib = (TableAttribute)queryType.GetCustomAttribute(typeof(TableAttribute));
            CacheHelper.TryAddTableName<TData>((attrib != null ? attrib.Name : queryType.Name));
        }
    }
}