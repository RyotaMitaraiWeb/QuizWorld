using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.Sorting
{
    public static class AllowedSortingOptions
    {
        public static readonly Dictionary<string, SortingCategories> Categories = new()
        {
            { "title", SortingCategories.Title },
            { "createdOn", SortingCategories.CreatedOn },
            { "updatedOn", SortingCategories.UpdatedOn },
        };

        public static readonly Dictionary<string, SortingOrders> Orders = new()
        {
            { "asc", SortingOrders.Ascending },
            { "desc", SortingOrders.Descending },
        };
    }
}
