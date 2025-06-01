using QuizWorld.Common.Constants.Sorting;
using QuizWorld.ViewModels.Quiz;
namespace QuizWorld.Web.Contracts.Quiz
{
    [Obsolete("Deprecated")]
    public interface IQuizServiceDeprecated
    {
        Task<int> CreateQuiz(CreateQuizViewModel quiz, string userId);
        Task<int> CreateQuiz(CreateQuizViewModel quiz, Guid userId);
        Task<QuizViewModel?> GetQuizById(int id);

        Task<int?> DeleteQuizById(int id);
        Task<int?> EditQuizById(int id, EditQuizViewModel quiz);
        Task<EditQuizFormViewModel?> GetQuizForEdit(int id);
        Task<CatalogueQuizViewModel> GetAllQuizzes(int page, SortingCategories category, SortingOrders order, int pageSize);
        Task<CatalogueQuizViewModel> GetUserQuizzes(string userId, int page, SortingCategories category, SortingOrders order, int pageSize);
        Task<CatalogueQuizViewModel> GetUserQuizzes(Guid userId, int page, SortingCategories category, SortingOrders order, int pageSize);
        Task<CatalogueQuizViewModel> GetQuizzesByQuery(string query, int page, SortingCategories category, SortingOrders order, int pageSize);
    }
}
