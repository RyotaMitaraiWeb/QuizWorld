using QuizWorld.ViewModels.Question;
using QuizWorld.Web.Contracts.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services.Quiz
{
    /// <summary>
    /// A service for retrieving correct answers of quizzes or questions.
    /// </summary>
    public class GradeService : IGradeService
    {
        public Task<GradedQuestionViewModel> GetCorrectAnswersForQuestionById(Guid questionId, int version)
        {
            throw new NotImplementedException();
        }

        public Task<GradedQuestionViewModel> GetCorrectAnswersForQuestionById(string questionId, int version)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GradedQuestionViewModel>> GetCorrectAnswersForQuestionsByQuizId(int quizId, int version)
        {
            throw new NotImplementedException();
        }
    }
}
