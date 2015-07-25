using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Dapper.Contrib.Linq
{
    static class DapperLinqContext<TData>
    {
        // Executes the expression tree that is passed to it. 
        internal static object Execute(IDbConnection connection, Expression expression, bool isEnumerable)
        {
            var qr = new QueryBuilder<TData>();
            qr.Visit(expression);
            return connection.Query<TData>(qr.Sql, qr.Parameters).AsQueryable();
        }
    }
}
