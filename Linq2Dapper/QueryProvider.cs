using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Contrib.Linq2Dapper.Exceptions;
using Dapper.Contrib.Linq2Dapper.Helpers;

namespace Dapper.Contrib.Linq2Dapper
{
    internal class QueryProvider<TData> : IQueryProvider
    {
        private readonly IDbConnection _connection;
        private readonly QueryBuilder<TData> _qb; 

        public QueryProvider(IDbConnection connection)
        {
            _connection = connection;
            _qb = new QueryBuilder<TData>();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Linq2Dapper<TData>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        // Queryable's collection-returning standard query operators call this method. 
        public IQueryable<TResult> CreateQuery<TResult>(Expression expression)
        {
            return new Linq2Dapper<TResult>(this, expression);
        }

        public object Execute(Expression expression)
        {
            return Execute(expression, false);
        }

        // Queryable's "single value" standard query operators call this method.
        public TResult Execute<TResult>(Expression expression)
        {
            var isEnumerable = (typeof(TResult) == typeof(IEnumerable<TData>));
            return (TResult)Execute(expression, isEnumerable);
        }

        // Executes the expression tree that is passed to it. 
        private object Execute(Expression expression, bool isEnumerable)
        {
            try
            {
                _qb.Visit(expression);
                var data = _connection.Query<TData>(_qb.Sql, _qb.Parameters);

                if (isEnumerable)
                    return data;

                return data.ElementAt(0);
            }
            catch (SqlException ex)
            {
                throw new InvalidQueryException(ex.Message);
            }
        }

    }
}
