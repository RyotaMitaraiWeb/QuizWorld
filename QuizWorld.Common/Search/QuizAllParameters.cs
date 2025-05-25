using QuizWorld.Common.Constants.Sorting;

namespace QuizWorld.Common.Search
{
    /// <summary>
    /// Sorting options for the "/all" endpoint
    /// </summary>
    public class QuizAllParameters
    {
        public int? Page { get; set; } = 1;
        public SortingOrders Order { get; set; } = SortingOrders.Ascending;
        public SortingCategories SortBy { get; set; } = SortingCategories.Title;
    }
}
