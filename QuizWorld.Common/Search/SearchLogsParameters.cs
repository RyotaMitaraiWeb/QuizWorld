using QuizWorld.Common.Constants.Sorting;

namespace QuizWorld.Common.Search
{
    public class SearchLogsParameters
    {
        public int Page { get; set; } = DefaultSearchParameters.Page;
        public SortingOrders Order {  get; set; } = DefaultSearchParameters.SortOrder;
        public int PageSize { get; set; } = 20;
    }
}
