using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Dapper.Contrib.Linq.Extensions
{
	public static class LinqExtensions
	{
        //public static Query<T> AsCacheable<T>(this IQueryable<T> query, long id) {
        //    QueryProvider provider = ProviderFinder.Find(query.Expression);
        //    return AsCacheable<T>(query, provider, id);
        //}
        //public static Query<T> AsCacheable<T>(this IQueryable<T> query, QueryProvider provider, long id) {			
        //    return new Query<T>(provider: provider, expression: query.Expression, id: id);			
        //}

		//public static IQueryable<T> AsLinqToObjectsQueryable<T>(this Query<T> query) {
		//    IQueryProvider provider = new object[0].AsQueryable().Provider;
		//    return provider.CreateQuery<T>(query.Expression);			
		//}

		/// <summary>
		/// Casts a collection, at runtime, to a generic (or strongly-typed) collection.
		/// </summary>
		public static IEnumerable<dynamic> Cast(this System.Collections.IEnumerable sourceobjects, Type elementType) {
			IQueryable queryable = sourceobjects.AsQueryable();
			MethodCallExpression castExpression =
				Expression.Call(typeof(Queryable), "Cast", new Type[] { elementType }, Expression.Constant(queryable));
			var lambdaCast = Expression.Lambda(castExpression, Expression.Parameter(typeof(System.Collections.IEnumerable)));
			dynamic castresults = lambdaCast.Compile().DynamicInvoke(new object[] { queryable });
			return Enumerable.ToArray(castresults);
		}		
		/// <summary>
		/// Allows duplicates to exist from the first set if present before the union, but excludes any elements
		/// from the second set which already exist in the left set during the union. This differs from the standard .NET union
		/// because it will eliminate duplicates from the left set if any exist before the union.
		/// 
		/// This performs approximately 1/100 as many comparisons as the standard .NET Union,
		/// and faster than using the .NET HashSet for the purpose of union.
		/// 
		/// </summary>
		/// <typeparam name="TSource">element Type of the first and second collections</typeparam>
		/// <param name="second">second collection to union with</param>
		/// <param name="comparer">IComparer</param>
		/// <returns></returns>
		public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second,
			IComparer<TSource> comparer) {

			int index = -1;
			List<TSource> third = new List<TSource>(first);	//can also use SortedList<Tkey,Tvalue>
			third.Sort(comparer);			
			foreach (TSource element in second) {				
				index = third.BinarySearch(element, comparer);
				if (index < 0) {			
					third.Add(element);
				}
			}
			return third;
		}

		/// <summary>
		/// .NET default impl. doesn't automatically use your impl. of IComparable or IEquatable
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="first"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		//public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer) {
		//    int index = -1;
		//}
		public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> first, IComparer<TSource> comparer) {
			SortedSet<TSource> second = new SortedSet<TSource>(comparer: comparer);	//can also use SortedList<Tkey,Tvalue>
			
			foreach (TSource element in first) {
				if (!second.Contains(element)) {
					second.Add(element);
				}
			}
			return second;
		}


        /// <summary>		
        /// </summary>
        /// <typeparam name="TSource">element Type of the first and second collections</typeparam>
        /// <param name="second">second collection to union with</param>
        /// <param name="comparison">IComparer</param>
        /// <returns></returns>
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second,
            Comparison<TSource> comparison)
        {
            IEqualityComparer<TSource> eqcomparer = new EqualityComparer<TSource>(comparison);
            HashSet<TSource> third = new HashSet<TSource>(first);	//can also use SortedList<Tkey,Tvalue>			
            foreach (TSource element in second)
            {
                if (!third.Contains(element))
                {
                    third.Add(element);
                }
            }
            return third;
        }

        /// <summary>
        /// .NET default impl. doesn't automatically use your impl. of IComparable or IEquatable
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="first"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>		
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> first, Comparison<TSource> comparison)
        {
            IEqualityComparer<TSource> eqcomparer = new EqualityComparer<TSource>(comparison);
            HashSet<TSource> second = new HashSet<TSource>(eqcomparer);	//can also use SortedList<Tkey,Tvalue>			
            foreach (TSource element in first)
            {
                if (!second.Contains(element))
                {
                    second.Add(element);
                }
            }
            return second;
        }

        public class EqualityComparer<T> : IEqualityComparer<T>
        {
            Comparison<T> comparison;
            public EqualityComparer(Comparison<T> comparison)
            {
                this.comparison = comparison;
            }
            #region IEqualityComparer<T> Members

            public bool Equals(T x, T y)
            {
                return this.comparison(x, y) == 0;
            }

            public int GetHashCode(T obj)
            {
                return base.GetHashCode();
            }

            #endregion
        }
	}

	
	
}
