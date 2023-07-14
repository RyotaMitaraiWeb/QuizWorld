using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Extensions
{
    public static class NormalizationExtensions
    {
        /// <summary>
        /// Uppercases the string's letter and removes all spaces. This is useful for
        /// providing a case-insensitive searching and sorting.
        /// </summary>
        /// <param name="input">The string to be normalized</param>
        /// <returns>A string with all letters uppercased and spaces removed</returns>
        public static string Normalized(this string input)
        {
            return input.ToUpper().Replace(" ", "");
        }
    }
}
