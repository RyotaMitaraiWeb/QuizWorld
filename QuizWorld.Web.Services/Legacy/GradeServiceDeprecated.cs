using Microsoft.EntityFrameworkCore;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.Web.Contracts.Legacy;

namespace QuizWorld.Web.Services.Legacy
{
    [Obsolete]
    /// <summary>
    /// A service for retrieving correct answers of quizzes or questions.
    /// </summary>
    public class GradeServiceDeprecated : IGradeServiceDeprecated
    {
        private readonly IRepository repository;
        public GradeServiceDeprecated(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets the correct answers for the question with the given <paramref name="questionId"/> and the given <paramref name="version"/>.
        /// Throws InvalidOperationException if the quiz is not in instant mode.
        /// </summary>
        /// <param name="questionId">The ID of the question to be graded</param>
        /// <param name="version">The version of the question whose answers will be retrieved</param>
        /// <returns>A view model of a graded question or null if the question does not exist or the quiz is deleted</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(Guid questionId, int version)
        {
            var question = await repository
                .AllReadonly<Question>()
                .Where(q => q.Id == questionId && q.Version == version && !q.Quiz.IsDeleted)
                .Select(q => new GradedQuestionViewModel()
                {
                    Id = q.Id.ToString(),
                    Answers = q.Answers
                        .Where(a => a.Correct)
                        .Select(a => new AnswerViewModel()
                        {
                            Id = a.Id.ToString(),
                            Value = a.Value,
                        }),
                    InstantMode = q.Quiz.InstantMode,
                })
                .FirstOrDefaultAsync();

            if (question == null)
            {
                return null;
            }

            if (!question.InstantMode)
            {
                throw new InvalidOperationException("This question cannot be graded individually because the quiz is not in instant mode");
            }

            return question;
        }

        /// <summary>
        /// Gets the correct answers for the question with the given <paramref name="questionId"/> and the given <paramref name="version"/>.
        /// Throws InvalidOperationException if the quiz is not in instant mode.
        /// </summary>
        /// <param name="questionId">The ID of the question to be graded</param>
        /// <param name="version">The version of the question whose answers will be retrieved</param>
        /// <returns>A view model of a graded question or null if the question does not exist or the quiz is deleted</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(string questionId, int version)
        {
            bool isGuid = Guid.TryParse(questionId, out Guid id);
            if (!isGuid)
            {
                throw new ArgumentException("questionId is an invalid GUID");
            }

            return await GetCorrectAnswersForQuestionById(id, version);
        }

        /// <summary>
        /// Gets the correct answers for all questions with the given <paramref name="version"/> of the quiz with the given <paramref name="quizId"/>.
        /// Throws InvalidOperationException if the quiz is in instant mode.
        /// </summary>
        /// <param name="quizId">The ID of the quiz</param>
        /// <param name="version">The version of the questions that have to be retrieved.</param>
        /// <returns>A list of graded questions or null if the list is empty.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IEnumerable<GradedQuestionViewModel>?> GetCorrectAnswersForQuestionsByQuizId(int quizId, int version)
        {
            var questions = await repository
                .AllReadonly<Question>()
                .Where(q => q.QuizId == quizId && q.Version == version && !q.Quiz.IsDeleted)
                .Select(q => new GradedQuestionViewModel()
                {
                    Id = q.Id.ToString(),
                    Answers = q.Answers
                        .Where(a => a.Correct)
                        .Select(a => new AnswerViewModel()
                        {
                            Id = a.Id.ToString(),
                            Value = a.Value,
                        }),
                    InstantMode = q.Quiz.InstantMode,
                })
                .ToListAsync();

            if (!questions.Any())
            {
                return null;
            }

            if (questions.First().InstantMode)
            {
                throw new InvalidOperationException("You cannot grade this quiz's questions at once because the quiz is in instant mode");
            }

            return questions;
        }
    }
}
