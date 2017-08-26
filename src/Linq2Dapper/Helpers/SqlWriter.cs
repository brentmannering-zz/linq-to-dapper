using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Contrib.Linq2Dapper.Helpers
{
    internal class SqlWriter<TData>
    {
        private StringBuilder _selectStatement;
        private readonly StringBuilder _joinTable;
        private readonly StringBuilder _whereClause;
        private readonly StringBuilder _orderBy;

        private int _nextParameter;

        private string _parameter
        {
            get { return string.Format("ld__{0}", _nextParameter += 1); }
        }

        internal Type SelectType;
        internal bool NotOperater;
        internal int TopCount;
        internal bool IsDistinct;

        internal DynamicParameters Parameters { get; private set; }

        internal string Sql
        {
            get
            {
                SelectStatement();
                return _selectStatement.ToString();
            }
        }

        internal SqlWriter()
        {
            Parameters = new DynamicParameters();
            _joinTable = new StringBuilder();
            _whereClause = new StringBuilder();
            _orderBy = new StringBuilder();
            SelectType = typeof(TData);
            GetTypeProperties();
        }

        private void GetTypeProperties()
        {
            QueryHelper.GetTypeProperties(typeof (TData));
        }

        private void SelectStatement()
        {
            var primaryTable = CacheHelper.TryGetTable<TData>();
            var selectTable = (SelectType != typeof(TData)) ? CacheHelper.TryGetTable(SelectType) : primaryTable;

            _selectStatement = new StringBuilder();

            _selectStatement.Append("SELECT ");

            if (TopCount > 0)
                _selectStatement.Append("TOP(" + TopCount + ") ");

            if (IsDistinct)
                _selectStatement.Append("DISTINCT ");

            for (int i = 0; i < selectTable.Columns.Count; i++)
            {
                var x = selectTable.Columns.ElementAt(i);
                _selectStatement.Append(string.Format("{0}.[{1}]", selectTable.Identifier, x.Value));

                if ((i + 1) != selectTable.Columns.Count)
                    _selectStatement.Append(",");

                _selectStatement.Append(" ");
            }

            _selectStatement.Append(string.Format("FROM [{0}] {1}", primaryTable.Name, primaryTable.Identifier));
            _selectStatement.Append(WriteClause());
        }

        private string WriteClause()
        {
            var clause = string.Empty;

            // JOIN
            if (!string.IsNullOrEmpty(_joinTable.ToString()))
                clause += _joinTable;

            // WHERE
            if (!string.IsNullOrEmpty(_whereClause.ToString()))
                clause += " WHERE " + _whereClause;

            //ORDER BY
            if (!string.IsNullOrEmpty(_orderBy.ToString()))
                clause += " ORDER BY " + _orderBy;

            return clause;
        }

        internal void WriteOrder(string name, bool descending)
        {
            var order = new StringBuilder();
            order.Append(name);
            if (descending) order.Append(" DESC");
            if (!string.IsNullOrEmpty(_orderBy.ToString())) order.Append(", ");
            _orderBy.Insert(0, order);
        }

        internal void WriteJoin(string joinToTableName, string joinToTableIdentifier, string primaryJoinColumn, string secondaryJoinColumn)
        {
            _joinTable.Append(string.Format(" JOIN [{0}] {1} ON {2} = {3}", joinToTableName, joinToTableIdentifier, primaryJoinColumn, secondaryJoinColumn));
        }

        internal void Write(object value)
        {
            _whereClause.Append(value);
        }

        internal void Parameter(object val)
        {
            if (val == null)
            {
                Write("NULL");
                return;
            }

            var param = _parameter;
            Parameters.Add(param, val);

            Write("@" + param);
        }

        internal void AliasName(string aliasName)
        {
            Write(aliasName);
        }

        internal void ColumnName(string columnName)
        {
            Write(columnName);
        }

        internal void IsNull()
        {
            Write(" IS");
            if (!NotOperater)
                Write(" NOT");
            Write(" NULL");
            NotOperater = false;
        }

        internal void IsNullFunction()
        {
            Write("ISNULL");
        }

        internal void Like()
        {
            if (NotOperater)
                Write(" NOT");
            Write(" LIKE ");
            NotOperater = false;
        }

        internal void In()
        {
            if (NotOperater)
                Write(" NOT");
            Write(" IN ");
            NotOperater = false;
        }

        internal void Operator()
        {
            Write(QueryHelper.GetOperator((NotOperater) ? ExpressionType.NotEqual : ExpressionType.Equal));
            NotOperater = false;
        }

        internal void Boolean(bool op)
        {
            Write((op ? " <> " : " = ") + "0");
        }

        internal void OpenBrace()
        {
            Write("(");
        }

        internal void CloseBrace()
        {
            Write(")");
        }

        internal void WhiteSpace()
        {
            Write(" ");
        }

        internal void Delimiter()
        {
            Write(", ");
        }

        internal void LikePrefix()
        {
            Write("'%' + ");
        }

        internal void LikeSuffix()
        {
            Write("+ '%'");
        }

        internal void EmptyString()
        {
            Write("''");
        }
    }
}
