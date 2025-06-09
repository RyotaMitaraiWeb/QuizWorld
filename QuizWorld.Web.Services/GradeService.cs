using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Errors;
using QuizWorld.Common.Result;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.Web.Contracts;
using static QuizWorld.Common.Errors.GradeError;

namespace QuizWorld.Web.Services
{
    public class GradeService(IRepository repository) : IGradeService
    {
        private readonly IRepository _repository = repository;

        public async Task<Result<GradedQuestionViewModel, GradeQuestionError>> GradeQuestion(string questionId, int version)
        {
            var question = await _repository
                .AllReadonly<Question>(question =>
                    question.Id == Guid.Parse(questionId)
                    && !question.Quiz.IsDeleted
                    && question.Version == version
                )
                
                .Select(question => new GradedQuestionViewModel()
                {
                    Id = question.Id.ToString(),
                    InstantMode = question.Quiz.InstantMode,
                    Answers = question.Answers
                        .Where(answer => answer.Correct)
                        .Select(answer => new AnswerViewModel()
                        {
                            Id = answer.Id.ToString(),
                            Value = answer.Value,
                        
                        })
                })
                .FirstOrDefaultAsync();

            if (question is null)
            {
                return Result<GradedQuestionViewModel, GradeQuestionError>
                    .Failure(GradeQuestionError.QuestionOrVersionDoesNotExist);
            }

            if (!question.InstantMode)
            {
                return Result<GradedQuestionViewModel, GradeQuestionError>
                    .Failure(GradeQuestionError.IsNotInstantMode);
            }

            return Result<GradedQuestionViewModel, GradeQuestionError>
                .Success(question);
        }

        public async Task<Result<IList<GradedQuestionViewModel>, GradeEntireQuizError>> GradeQuiz(int quizId, int version)
        {
            var questions = await _repository
                .AllReadonly<Question>(question =>
                    question.Version == version
                    && question.Quiz.Id == quizId
                    && !question.Quiz.IsDeleted)
                .Select(question => new GradedQuestionViewModel()
                {
                    Id = question.Id.ToString(),
                    InstantMode = question.Quiz.InstantMode,
                    Answers = question.Answers
                        .Where(answer => answer.Correct)
                        .Select(answer => new AnswerViewModel()
                        {
                            Id = answer.Id.ToString(),
                            Value = answer.Value,

                        })
                })
                .ToListAsync();

            if (questions.Count == 0)
            {
                return Result<IList<GradedQuestionViewModel>, GradeEntireQuizError>
                    .Failure(GradeEntireQuizError.QuizOrVersionDoesNotExist);
            }

            bool instantMode = questions.First().InstantMode;
            if (instantMode)
            {
                return Result<IList<GradedQuestionViewModel>, GradeEntireQuizError>
                    .Failure(GradeEntireQuizError.IsInstantMode);
            }

            return Result<IList<GradedQuestionViewModel>, GradeEntireQuizError>
                .Success(questions);
        }
    }
}
