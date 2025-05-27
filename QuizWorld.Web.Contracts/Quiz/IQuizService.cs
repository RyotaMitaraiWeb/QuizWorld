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
    }
}
