using QuizWorld.ViewModels.Question;

namespace QuizWorld.Web.Contracts.Quiz
{
    [Obsolete]
    public interface IGradeServiceDeprecated
    {
        Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(Guid questionId, int version);
        Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(string questionId, int version);

        Task<IEnumerable<GradedQuestionViewModel>?> GetCorrectAnswersForQuestionsByQuizId(int quizId, int version);
    }
}
