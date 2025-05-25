using QuizWorld.Common.Search;
using QuizWorld.ViewModels.Quiz;

namespace QuizWorld.Web.Contracts.Quiz
{
    public interface IQuizService
    {
        Task<CatalogueQuizViewModel> Search(QuizSearchParameterss parameters);
    }
}
