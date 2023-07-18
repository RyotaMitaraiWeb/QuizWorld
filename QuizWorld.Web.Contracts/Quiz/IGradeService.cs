using QuizWorld.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Contracts.Quiz
{
    public interface IGradeService
    {
        Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(Guid questionId, int version);
        Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(string questionId, int version);

        Task<IEnumerable<GradedQuestionViewModel>?> GetCorrectAnswersForQuestionsByQuizId(int quizId, int version);
    }
}
