using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Dapper.Contrib.Linq2Dapper.Exceptions;
using Dapper.Contrib.Linq2Dapper.Helpers;
using System.Collections;

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
            Type elementType = TypeHelper.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Linq2Dapper<TData>).MakeGenericType(elementType), this, expression);
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
            return Query(expression);
        }

        // Queryable's "single value" standard query operators call this method.
        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Query(expression, typeof(IEnumerable).IsAssignableFrom(typeof(TResult)));
        }
        
        // Executes the expression tree that is passed to it. 
        private object Query(Expression expression, bool isEnumerable = false)
        {
            try
            {
                if (_connection.State != ConnectionState.Open) _connection.Open();

                _qb.Evaluate(expression);
                var data = _connection.Query<TData>(_qb.Sql, _qb.Parameters);

                if (isEnumerable) return data;
                return data.ElementAt(0);
            }
            catch (SqlException ex)
            {
                throw new InvalidQueryException(ex.Message + " | " + _qb.Sql);
            }
            finally
            {
                _connection.Close();
            }
        }

    }
}
