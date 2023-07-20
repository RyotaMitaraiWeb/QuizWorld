using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Extensions
{
    /// <summary>
    /// Extensions for queries of type IQueryable
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Shuffles an IEnumerable collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection to be shuffled</param>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> collection)
        {
            var rand = new Random();
            var array = collection.ToArray();
            int length = array.Length;

            for (int i = length - 1; i > 0; i--)
            {
                int randomIndex = rand.Next(i + 1);
                (array[i], array[randomIndex]) = (array[randomIndex], array[i]);
            }

            return array.AsEnumerable();
        }

        /// <summary>
        /// Sorts the <paramref name="query"/> based on the provided parameters.
        /// </summary>
        /// <param name="query">The query to be sorted</param>
        /// <param name="category">
        /// The category by which the <paramref name="query"/> will be sorted.
        /// If passed "title", the categories will be sorted by their normalized names,
        /// rendering the sorting case insensitive.
        /// </param>
        /// <param name="order">The order by which the query will be sorted</param>
        /// <returns>A sorted query</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IOrderedQueryable<Quiz> SortByOptions(this IQueryable<Quiz> query, SortingCategories category, SortingOrders order)
        {

            return category switch
            {
                SortingCategories.Title => query.SortByOrder(q => q.NormalizedTitle, order),
                SortingCategories.CreatedOn => query.SortByOrder(q => q.CreatedOn, order),
                SortingCategories.UpdatedOn => query.SortByOrder(q => q.UpdatedOn, order),
                _ => throw new ArgumentException("Category is invalid"),
            };
        }

        /// <summary>
        /// Sorts the <paramref name="query"/> in an ascending or descending order by the provided <paramref name="expression"/>
        /// </summary>
        /// <typeparam name="TClass">The class of the collection</typeparam>
        /// <typeparam name="TValue">The type of the value in the expression</typeparam>
        /// <param name="query">The query to be ordered</param>
        /// <param name="expression">The expression by which the query will be ordered</param>
        /// <param name="order">The order in which the query will be sorted</param>
        /// <returns>A query that is ordered in an ascending or descending order.</returns>
        public static IOrderedQueryable<TClass> SortByOrder<TClass, TValue>(this IQueryable<TClass> query, Expression<Func<TClass, TValue>> expression, SortingOrders order)
        {
            if (order == SortingOrders.Ascending)
            {
                return query.OrderBy(expression);
            }
            else
            {
                return query.OrderByDescending(expression);
            }
        }
    }
}
