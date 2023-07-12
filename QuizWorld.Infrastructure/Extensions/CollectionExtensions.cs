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
    }
}
