using Microsoft.EntityFrameworkCore;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<int> CreateQuiz(CreateQuizViewModel quiz, string userId)
        {
            var id = new Guid(userId);
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

        public Task<int> DeleteQuizById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> EditQuizById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogueQuizViewModel>> GetAllQuizzes(int page, string category, string order)
        {
            throw new NotImplementedException();
        }

        public Task<QuizViewModel?> GetQuizById(int id)
        {
            var quiz = this.repository
                .AllReadonly<Quiz>(q => q.Id == id)
                .Select(q => new QuizViewModel()
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Version = q.Version,
                    InstantMode = q.InstantMode,
                    Questions = q.Questions
                        .Where(question => question.Version == q.Version)
                        .OrderBy(question => question.Order)
                        .Select(question => new QuestionViewModel()
                        {
                            Prompt = question.Prompt,
                            Id = question.Id.ToString(),
                            Type = question.QuestionType.ShortName,
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

        public Task<IEnumerable<CatalogueQuizViewModel>> GetQuizzesByQuery(string query, int page, string category, string order)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogueQuizViewModel>> GetUserQuizzes(string userId, int page, string category, string order)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CatalogueQuizViewModel>> GetUserQuizzes(Guid userId, int page, string category, string order)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<Question> CreateQuestions(IEnumerable<CreateQuestionViewModel> questionModels)
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
                    Version = 1,
                    Order = i + 1,
                    Answers = this.CreateAnswers(question.Answers).ToList()
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
