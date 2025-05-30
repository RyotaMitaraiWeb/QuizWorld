using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Errors;
using QuizWorld.Common.Result;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.Quiz;
using System.Linq.Expressions;
using static QuizWorld.Common.Errors.QuizError;

namespace QuizWorld.Web.Services
{
    public class QuizService(IRepository quizRepository) : IQuizService
    {
        private readonly IRepository _quizRepository = quizRepository;
        public async Task<CatalogueQuizViewModel> SearchAsync(QuizSearchParameterss parameters)
        {
            Expression<Func<Quiz, bool>> predicate = BuildPredicate(parameters);

            IQueryable<Quiz> query = _quizRepository
                    .AllReadonly(predicate);
            int count = await query.CountAsync();

            var quizzes = await query
                .Paginate(parameters.Page, parameters.PageSize)
                .SortByOptions(
                    category: parameters.SortBy, order: parameters.Order)
                .Select(q => new CatalogueQuizItemViewModel()
                {
                    Title = q.Title,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn,
                    Id = q.Id,
                    InstantMode = q.InstantMode,
                    Description = q.Description,
                })
                .ToListAsync();

            return new CatalogueQuizViewModel()
            {
                Total = count,
                Quizzes = quizzes,
            };
        }

        public async Task<Result<QuizViewModel, QuizGetError>> GetAsync(int quizId)
        {
            QuizViewModelWithDeleted? quiz = await FindQuiz(quizId);

            if (quiz is null)
            {
                return Result<QuizViewModel, QuizGetError>
                    .Failure(QuizGetError.DoesNotExist);
            }

            if (quiz.IsDeleted)
            {
                return Result<QuizViewModel, QuizGetError>
                    .Failure(QuizGetError.IsDeleted);
            }

            var quizModel = new QuizViewModel()
            {
                Id = quizId,
                Title = quiz.Title,
                CreatedOn = quiz.CreatedOn,
                UpdatedOn = quiz.UpdatedOn,
                Description = quiz.Description,
                CreatorId = quiz.CreatorId.ToString(),
                InstantMode = quiz.InstantMode,
                Questions = quiz.Questions,
                Version = quiz.Version,
                CreatorUsername = quiz.CreatorUsername,
            };

            return Result<QuizViewModel, QuizGetError>
                .Success(quizModel);
        }

        public async Task<int> CreateAsync(CreateQuizViewModel quiz, string userId, DateTime creationDate)
        {
            Quiz newQuiz = new ()
            {
                Title = quiz.Title,
                NormalizedTitle = quiz.Title.ToLower(),
                Description = quiz.Description,
                InstantMode = quiz.InstantMode,
                Version = 1,
                CreatorId = new Guid(userId),
                CreatedOn = creationDate,
                UpdatedOn = creationDate,
                IsDeleted = false,
                Questions = BuildQuestions([.. quiz.Questions], 1)
                
            };

            await _quizRepository.AddAsync(newQuiz);
            await _quizRepository.SaveChangesAsync();
            return newQuiz.Id;
        }

        public async Task<EditQuizFormViewModel> GetForEditAsync(int quizId)
        {
            EditQuizFormViewModel quiz = await _quizRepository
                .AllReadonly<Quiz>(quiz => quiz.Id == quizId && !quiz.IsDeleted)
                .Select(quiz => new EditQuizFormViewModel()
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    Description = quiz.Description,
                    Questions = quiz.Questions.Select(question => new EditQuestionFormViewModel()
                    {
                        Prompt = question.Prompt,
                        Notes = question.Notes,
                        Type = question.QuestionType.ShortName,
                        Order = question.Order,
                        Answers = question.Answers.Select(answer => new EditAnswerFormViewModel()
                        {
                            Value = answer.Value,
                            Correct = answer.Correct,
                        }),
                    })
                })
                .FirstAsync();

            return quiz;
        }

        public async Task EditAsync(int quizId, EditQuizViewModel quiz, DateTime updatedOn)
        {
            Quiz quizToEdit = await _quizRepository.GetByIdAsync<Quiz>(quizId);
            int newVersion = quizToEdit.Version + 1;

            quizToEdit.Title = quiz.Title;
            quizToEdit.NormalizedTitle = quiz.Title.ToLower();
            quizToEdit.Description = quiz.Description;
            quizToEdit.Version = newVersion;
            quizToEdit.UpdatedOn = updatedOn;

            var newQuestions = BuildQuestions([.. quiz.Questions], newVersion);
            foreach (var question in newQuestions)
            {
                quizToEdit.Questions.Add(question);
            }

            await _quizRepository.SaveChangesAsync();
            return;
        }

        public async Task DeleteAsync(int quizId)
        {
            await _quizRepository.DeleteAsync<Quiz>(quizId);
            await _quizRepository.SaveChangesAsync();
        }
        private static Expression<Func<Quiz, bool>> BuildPredicate(QuizSearchParameterss parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters.Author))
            {
                return q => !q.IsDeleted
                && q.NormalizedTitle.Contains(parameters.Title.ToLower())
                && q.Creator.NormalizedUserName == parameters.Author.ToUpper();
            }
            else
            {
                return q => !q.IsDeleted 
                && q.NormalizedTitle.Contains(parameters.Title.ToLower());
            }
        }

        private static List<Question> BuildQuestions(List<CreateQuestionViewModel> questions, int quizVersion)
        {
            List<Question> list = [];

            int end = questions.Count;
            for (int i = 0; i < end; i++)
            {
                int order = i + 1;
                CreateQuestionViewModel question = questions[i];
                Question newQuestion = new()
                {
                    Prompt = question.Prompt,
                    Notes = question.Notes,
                    QuestionTypeId = ((int)question.Type) + 1,
                    Order = order,
                    Version = quizVersion,
                    Answers = BuildAnswers(question.Answers),
                };

                list.Add(newQuestion);
            }

            return list;
        }

        private static List<Answer> BuildAnswers(IEnumerable<CreateAnswerViewModel> answers)
        {
            return [.. answers.Select(answer => new Answer()
            {
                Correct = answer.Correct,
                Value = answer.Value,
            })];
        }

        private async Task<QuizViewModelWithDeleted?> FindQuiz(int quizId)
        {
            return await _quizRepository
                .AllReadonly<Quiz>(q => q.Id == quizId)
                .Select(quiz => new QuizViewModelWithDeleted()
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    CreatedOn = quiz.CreatedOn,
                    UpdatedOn = quiz.UpdatedOn,
                    Description = quiz.Description,
                    InstantMode = quiz.InstantMode,
                    Questions = quiz.Questions
                        .Where(question => question.Version == quiz.Version)
                        .OrderBy(question => question.Order)
                        .Select(question => new QuestionViewModel()
                        {
                            Prompt = question.Prompt,
                            Id = question.Id.ToString(),
                            Type = question.QuestionType.ShortName,
                            Notes = question.Notes,
                            Answers = question.Answers
                                .Select(a => new AnswerViewModel()
                                {
                                    Value = a.Value,
                                    Id = a.Id.ToString(),
                                })
                                .Shuffle()
                        }),
                    IsDeleted = quiz.IsDeleted,
                    CreatorId = quiz.Creator.Id.ToString(),
                    CreatorUsername = quiz.Creator.UserName!,
                    Version = quiz.Version,

                })
                .FirstOrDefaultAsync();
        }
    }

    /// <summary>
    /// Used to check if the quiz is deleted, rather than not existing,
    /// to improve error tracing
    /// </summary>
    internal class QuizViewModelWithDeleted : QuizViewModel
    {
        public bool IsDeleted { get; set; }
    }
}