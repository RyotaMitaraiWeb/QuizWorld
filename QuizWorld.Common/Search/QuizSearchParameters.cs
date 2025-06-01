using QuizWorld.Common.Constants.Sorting;
using System.ComponentModel.DataAnnotations;

namespace QuizWorld.Common.Search
{
    public class QuizSearchParameterss
    {
        public int Page { get ; set; } = DefaultSearchParameters.Page;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public SortingOrders Order { get; set; } = DefaultSearchParameters.SortOrder;
        public SortingCategories SortBy { get; set; } = DefaultSearchParameters.SortBy;
        public int PageSize { get; set; } = DefaultSearchParameters.PageSize;
    }
}
