using QuizWorld.Common.Result;
using QuizWorld.Common.Search;
using QuizWorld.ViewModels.Quiz;
using static QuizWorld.Common.Errors.QuizError;

namespace QuizWorld.Web.Contracts.Quiz
{
    public interface IQuizService
    {
        Task<CatalogueQuizViewModel> SearchAsync(QuizSearchParameterss parameters);
        Task<Result<QuizViewModel, QuizGetError>> GetAsync(int quizId);
        Task<int> CreateAsync(CreateQuizViewModel quiz, string userId, DateTime creationDate);
        Task EditAsync(int quizId, EditQuizViewModel quiz, DateTime updatedOn);
        Task DeleteAsync(int quizId);
        Task<EditQuizFormViewModel> GetForEditAsync(int quizId);
    }
}
