using QuizWorld.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static IOrderedQueryable<Quiz> SortByOptions(this IQueryable<Quiz> query, string category, string order)
        {
            if (order == "asc")
            {
                return category switch
                {
                    "title" => query.OrderBy(q => q.NormalizedTitle).ThenBy(q => q.CreatedOn),
                    "createdOn" => query.OrderBy(q => q.CreatedOn),
                    "updatedOn" => query.OrderBy(q => q.UpdatedOn),
                    _ => throw new ArgumentException("Category is invalid"),
                };
            }

            else if (order == "desc")
            {
                return category switch
                {
                    "title" => query.OrderByDescending(q => q.NormalizedTitle),
                    "createdOn" => query.OrderByDescending(q => q.CreatedOn),
                    "updatedOn" => query.OrderByDescending(q => q.UpdatedOn),
                    _ => throw new ArgumentException("Category is invalid"),
                };
            }
            else
            {
                throw new ArgumentException("Order is invalid");
            }
        }
    }
}
