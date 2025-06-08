using QuizWorld.Common.Constants.Sorting;

namespace QuizWorld.Common.Search
{
    public class SearchUsersParameters
    {
        public int Page { get; set; } = DefaultSearchParameters.Page;
        public int PageSize {  get; set; } = DefaultSearchParameters.PageSize;

        public string? Username { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
        public SortingOrders Order { get; set; } = DefaultSearchParameters.SortOrder;
    }
}
