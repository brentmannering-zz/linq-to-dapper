using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dapper.Contrib.Linq2Dapper.Extensions
{
    public static class DapperExtensions
    {
        public static IQueryable<T> Query<T>(this IDbConnection dbConnection, Expression<Func<T, bool>> expression = null)
        {
            return new Linq2Dapper<T>(dbConnection, expression: expression);
        }

        public static async Task<IQueryable<T>> QueryAsync<T>(this IDbConnection dbConnection, Expression<Func<T, bool>> expression = null)
        {
            return await Task.Run(() => new Linq2Dapper<T>(dbConnection, expression: expression));
        }
    }
}
