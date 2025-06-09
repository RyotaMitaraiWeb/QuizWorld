using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Question;
using static QuizWorld.Common.Errors.GradeError;

namespace QuizWorld.Web.Contracts
{
    public interface IGradeService
    {
        Task<Result<GradedQuestionViewModel, GradeQuestionError>> GradeQuestion(string questionId, int version);
        Task<Result<IList<GradedQuestionViewModel>, GradeEntireQuizError>> GradeQuiz(int quizId, int version);
    }
}
