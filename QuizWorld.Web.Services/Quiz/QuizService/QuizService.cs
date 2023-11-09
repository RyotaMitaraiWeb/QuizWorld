using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.Quiz;

namespace QuizWorld.Web.Services.QuizService
{
    /// <summary>
    /// A service for interacting with quizzes in the database.
    /// </summary>
    public class QuizService : IQuizService
    {
        private readonly IRepository repository;
        public QuizService(IRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Creates a quiz based on the passed model and adds it to the database.
        /// </summary>
        /// <param name="quiz">The model of the quiz that will be created</param>
        /// <param name="userId">The ID of the creator. This will be converted to a Guid.</param>
        /// <returns>The ID of the created quiz if successful.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<int> CreateQuiz(CreateQuizViewModel quiz, string userId)
        {
            bool isGuid = Guid.TryParse(userId, out var id);
            if (!isGuid)
            {
                throw new InvalidOperationException("User ID is malformed (not a GUID)");
            }

            return await this.CreateQuiz(quiz, id);
        }

        /// <summary>
        /// Creates a quiz based on the passed model and adds it to the database.
        /// </summary>
        /// <param name="quiz">The model of the quiz that will be created</param>
        /// <param name="userId">The ID of the creator</param>
        /// <returns>The ID of the created quiz if successful.</returns>
        public async Task<int> CreateQuiz(CreateQuizViewModel quiz, Guid userId)
        {
            var questions = this.CreateQuestions(quiz.Questions);
            var date = DateTime.Now;

            var entity = new Quiz()
            {
                Title = quiz.Title,
                NormalizedTitle = quiz.Title.ToUpper(),
                Description = quiz.Description,
                Version = 1,
                InstantMode = quiz.InstantMode,
                CreatedOn = date,
                UpdatedOn = date,
                IsDeleted = false,
                CreatorId = userId,
                Questions = questions.ToList(),
            };

            await this.repository.AddAsync(entity);
            await this.repository.SaveChangesAsync();

            return entity.Id;
        }

        /// <summary>
        /// Sets the "IsDeleted" column of the quiz to true.
        /// </summary>
        /// <param name="id">The ID of the quiz</param>
        /// <returns>The ID of the quiz if successful or null if quiz does not exist or has already been deleted.</returns>
        public async Task<int?> DeleteQuizById(int id)
        {
            var quiz = await this.repository.GetByIdAsync<Quiz>(id);
            if (quiz == null || quiz.IsDeleted)
            {
                return null;
            }

            quiz.IsDeleted = true;

            await this.repository.SaveChangesAsync();
            return id;
        }

        /// <summary>
        /// Retrieves a quiz with all its data needed to edit it.
        /// </summary>
        /// <param name="id">The ID of the quiz</param>
        /// <returns>A view model with all the data to be populated in the edit quiz form
        /// or null if the quiz does not exist or is deleted</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<EditQuizFormViewModel?> GetQuizForEdit(int id)
        {
            return this.repository.AllReadonly<Quiz>()
                .Where(quiz => quiz.Id == id && !quiz.IsDeleted)
                .Select(quiz => new EditQuizFormViewModel()
                {
                    Id = quiz.Id,
                    Title = quiz.Title,
                    Description = quiz.Description,
                    Questions = quiz.Questions
                        .Where(question => question.Version == quiz.Version)
                        .OrderBy(question => question.Order)
                        .Select(question => new EditQuestionFormViewModel()
                        {
                            Prompt = question.Prompt,
                            Type = question.QuestionType.Type.ToString(),
                            Order = question.Order,
                            Answers = question.Answers
                                .Select(answer => new EditAnswerFormViewModel()
                                {
                                    Value = answer.Value,
                                    Correct = answer.Correct,
                                })

                        })
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int?> EditQuizById(int id, EditQuizViewModel quiz)
        {
            var quizEntity = await this.repository.GetByIdAsync<Quiz>(id);
            if (quizEntity == null || quizEntity.IsDeleted)
            {
                return null;
            }

            quizEntity.Title = quiz.Title;
            quizEntity.NormalizedTitle = quiz.Title.Normalized();
            quizEntity.Version++;
            quizEntity.Description = quiz.Description;
            quizEntity.UpdatedOn = DateTime.Now;

            var questions = this.CreateQuestions(quiz.Questions, quizEntity.Version);
            
            foreach (var question in questions)
            {
                quizEntity.Questions.Add(question);
            }

            await this.repository.SaveChangesAsync();
            return id;

        }

        /// <summary>
        /// Retrieves a catalogue of quizzes that have not been deleted and the total amount of quizzes.
        /// Parameters control how the result will be paginated and sorted.
        /// </summary>
        /// <param name="page">The requested page</param>
        /// <param name="category">Category by which the result will be sorted</param>
        /// <param name="order">The order by which the result will be sorted</param>
        /// <param name="pageSize">The number of quizzes that will be retrieved</param>
        /// <returns>A model that contains the total amount of non-deleted quizzes and the catalogue</returns>
        public async Task<CatalogueQuizViewModel> GetAllQuizzes(int page, SortingCategories category, SortingOrders order, int pageSize = 6)
        {
            var query = this.repository
                .AllReadonly<Quiz>()
                .Where(q => !q.IsDeleted);


            int total = await query.CountAsync();
            var quizzes = await query
                .SortByOptions(category, order)
                .Paginate(page, pageSize)
                .Select(q => new CatalogueQuizItemViewModel()
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    InstantMode = q.InstantMode,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn,
                })
                .ToListAsync();

            var catalogue = new CatalogueQuizViewModel()
            {
                Total = total,
                Quizzes = quizzes,
            };

            return catalogue;
        }

        public Task<QuizViewModel?> GetQuizById(int id)
        {
            var quiz = this.repository
                .AllReadonly<Quiz>(q => q.Id == id && !q.IsDeleted)
                .Select(q => new QuizViewModel()
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Version = q.Version,
                    InstantMode = q.InstantMode,
                    CreatorId = q.CreatorId.ToString(),
                    CreatorUsername = q.Creator.UserName,
                    Questions = q.Questions
                        .Where(question => question.Version == q.Version)
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
                        })
                })
                .FirstOrDefaultAsync();

            return quiz;

        }

        /// <summary>
        /// Retrieves a paginated and sorted catalogue of quizzes whose title contains the given <paramref name="query"/> and the total amount of such quizzes.
        /// </summary>
        /// <param name="query">The string to be searched. The search is case insensitive and ignores spaces.</param>
        /// <param name="page">The current page</param>
        /// <param name="category">Category by which the result will be sorted</param>
        /// <param name="order">The order by which the result will be sorted</param>
        /// <param name="pageSize">The number of quizzes that will be retrieved</param>
        /// <returns>A model that contains the total amount of quizzes whose title contains the given <paramref name="query"/> and the catalogue</returns>
        public async Task<CatalogueQuizViewModel> GetQuizzesByQuery(string query, int page, SortingCategories category, SortingOrders order, int pageSize = 6)
        {
            var queryList = this.repository
                .AllReadonly<Quiz>()
                .Where(q => !q.IsDeleted && q.NormalizedTitle.Contains(query.Normalized()));

            int total = await queryList.CountAsync();

            var quizzes = await queryList
                .SortByOptions(category, order)
                .Paginate(page, pageSize)
                .Select(q => new CatalogueQuizItemViewModel()
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    InstantMode = q.InstantMode,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn,
                })
                .ToListAsync();

            var catalogue = new CatalogueQuizViewModel()
            {
                Total = total,
                Quizzes = quizzes,
            };

            return catalogue;
        }

        /// <summary>
        /// Returns a paginated and sorted catalogue of the quizzes that belong to the user with the given ID and a total count of the user's quizzes.
        /// </summary>
        /// <param name="userId">The ID of the user. The ID will be parsed into a GUID.</param>
        /// <param name="page">The current page</param>
        /// <param name="category">The category by which the result will be sorted</param>
        /// <param name="order">The order in which the result will be sorted</param>
        /// <param name="pageSize">The amount of quizzes to be retrieved</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<CatalogueQuizViewModel> GetUserQuizzes(string userId, int page, SortingCategories category, SortingOrders order, int pageSize = 6)
        {
            bool successfulParse = Guid.TryParse(userId, out Guid id);

            if (!successfulParse)
            {
                throw new ArgumentException("The provided ID is invalid");
            }

            return await this.GetUserQuizzes(id, page, category, order, pageSize);
        }

        /// <summary>
        /// Returns a paginated and sorted catalogue of the quizzes that belong to the user with the given ID and a total count of the user's quizzes.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="page">The current page</param>
        /// <param name="category">The category by which the result will be sorted</param>
        /// <param name="order">The order in which the result will be sorted</param>
        /// <param name="pageSize">The amount of quizzes to be retrieved</param>
        /// <returns></returns>
        public async Task<CatalogueQuizViewModel> GetUserQuizzes(Guid userId, int page, SortingCategories category, SortingOrders order, int pageSize = 6)
        {
            var query = this.repository
                .AllReadonly<Quiz>()
                .Where(q => q.CreatorId == userId && !q.IsDeleted);

            int total = await query.CountAsync();

            var quizzes = await query
                .SortByOptions(category, order)
                .Paginate(page, pageSize)
                .Select(q => new CatalogueQuizItemViewModel()
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    InstantMode = q.InstantMode,
                    CreatedOn = q.CreatedOn,
                    UpdatedOn = q.UpdatedOn,
                })
                .ToListAsync();

            var catalogue = new CatalogueQuizViewModel()
            {
                Total = total,
                Quizzes = quizzes,
            };

            return catalogue;

        }

        private IEnumerable<Question> CreateQuestions(IEnumerable<CreateQuestionViewModel> questionModels, int version = 1)
        {
            var questions = new List<Question>();
            var questionsArray = questionModels.ToArray();
            int end = questionsArray.Length;

            for (int i = 0; i < end; i++)
            {
                var question = questionsArray[i];
                int typeId = (int)question.Type + 1;

                var questionEntity = new Question()
                {
                    Prompt = question.Prompt,
                    QuestionTypeId = typeId,
                    Version = version,
                    Order = i + 1,
                    Answers = this.CreateAnswers(question.Answers).ToList(),
                    Notes = question.Notes,
                };

                questions.Add(questionEntity);
            }

            return questions;
        }

        private IEnumerable<Answer> CreateAnswers(IEnumerable<CreateAnswerViewModel> answerModels)
        {
            var answers = new List<Answer>();
            foreach (var answer in answerModels)
            {
                var answerEntity = new Answer()
                {
                    Value = answer.Value,
                    Correct = answer.Correct
                };

                answers.Add(answerEntity);
            }

            return answers;
        }
    }
}
