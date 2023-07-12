using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Services.QuizService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.QuizServiceInMemoryTests
{
    public class InMemory
    {
        public TestDB testDB;
        public Repository repository;
        public QuizService service;

        [SetUp]
        public void Setup()
        {
            this.testDB = new TestDB();
            this.repository = this.testDB.repository;
            this.service = new QuizService(this.repository);
        }

        [Test]
        public async Task Test_CreateQuizAddsAQuizToTheDatabaseIfPassedAUserGuid()
        {
            var quiz = this.CreateQuizModel();

            var result = await this.service.CreateQuiz(quiz, this.testDB.User.Id);
            Assert.That(result, Is.EqualTo(4));

            var entity = await this.repository.
                AllReadonly<Quiz>()
                .Where(entity => entity.Id == result)
                .Include(e => e.Questions)
                .ThenInclude(q => q.Answers)
                .Include(e => e.Questions)
                .ThenInclude(q => q.QuestionType)
                .FirstAsync();
            Assert.Multiple(() =>
            {
                Assert.That(entity.Title, Is.EqualTo(quiz.Title));
                Assert.That(entity.InstantMode, Is.True);
                Assert.That(entity.Description, Is.EqualTo(quiz.Description));
                Assert.That(entity.NormalizedTitle, Is.EqualTo(quiz.Title.ToUpper()));
                Assert.That(entity.CreatorId, Is.EqualTo(this.testDB.User.Id));
                Assert.That(entity.UpdatedOn, Is.EqualTo(entity.CreatedOn));

                var questions = entity.Questions.ToArray();
                Assert.That(questions[0].QuestionType.ShortName, Is.EqualTo(QuestionTypesShortNames.Text));
                Assert.That(questions[0].Prompt, Is.EqualTo("Demo prompt"));
                Assert.That(questions[0].Order, Is.EqualTo(1));
                Assert.That(questions[0].QuizId, Is.EqualTo(entity.Id));

                var answers = questions[0].Answers.ToArray();
                Assert.That(answers[0].QuestionId, Is.EqualTo(questions[0].Id));
                Assert.That(answers[0].Correct, Is.True);
                Assert.That(answers[0].Value, Is.EqualTo("demo answer"));
            });
        }

        [Test]
        public async Task Test_CreateQuizAddsAQuizToTheDatabaseIfPassedAStringUserId()
        {
            var quiz = this.CreateQuizModel();

            var result = await this.service.CreateQuiz(quiz, this.testDB.User.Id.ToString());
            Assert.That(result, Is.EqualTo(4));

            var entity = await this.repository.
                AllReadonly<Quiz>()
                .Where(entity => entity.Id == result)
                .Include(e => e.Questions)
                .ThenInclude(q => q.Answers)
                .Include(e => e.Questions)
                .ThenInclude(q => q.QuestionType)
                .FirstAsync();
            Assert.Multiple(() =>
            {
                Assert.That(entity.Title, Is.EqualTo(quiz.Title));
                Assert.That(entity.InstantMode, Is.True);
                Assert.That(entity.Description, Is.EqualTo(quiz.Description));
                Assert.That(entity.NormalizedTitle, Is.EqualTo(quiz.Title.ToUpper()));
                Assert.That(entity.CreatorId, Is.EqualTo(this.testDB.User.Id));
                Assert.That(entity.UpdatedOn, Is.EqualTo(entity.CreatedOn));

                var questions = entity.Questions.ToArray();
                Assert.That(questions[0].QuestionType.ShortName, Is.EqualTo(QuestionTypesShortNames.Text));
                Assert.That(questions[0].Prompt, Is.EqualTo("Demo prompt"));
                Assert.That(questions[0].Order, Is.EqualTo(1));
                Assert.That(questions[0].QuizId, Is.EqualTo(entity.Id));

                var answers = questions[0].Answers.ToArray();
                Assert.That(answers[0].QuestionId, Is.EqualTo(questions[0].Id));
                Assert.That(answers[0].Correct, Is.True);
                Assert.That(answers[0].Value, Is.EqualTo("demo answer"));
            });
        }

        [Test]
        public async Task Test_GetQuizByIdRetrievesTheQuizWithTheGivenIdAndIncludesOnlyTheQuestionsThatMatchItsVersion()
        {
            var entity = this.testDB.Quiz;
            var quiz = await this.service.GetQuizById(entity.Id);
            Assert.Multiple(() =>
            {
                Assert.That(quiz.Title, Is.EqualTo(entity.Title));
                Assert.That(quiz.Description, Is.EqualTo(entity.Description));
                Assert.That(quiz.InstantMode, Is.EqualTo(entity.InstantMode));
                Assert.That(quiz.Id, Is.EqualTo(entity.Id));
                Assert.That(quiz.Version, Is.EqualTo(entity.Version));

                var questions = quiz.Questions.ToArray();
                var answers = questions[0].Answers.ToArray();

                // The quiz has four in the datbase, one of which does not match the version
                Assert.That(questions, Has.Length.EqualTo(3));
                Assert.That(questions[0].Type, Is.EqualTo(QuestionTypesShortNames.SingleChoice));
                Assert.That(answers, Has.Length.EqualTo(2));
            });
        }

        [Test]
        public async Task Test_GetQuizByIdReturnsNullIfItCannotFindTheQuiz()
        {
            var quiz = await this.service.GetQuizById(0);
            Assert.That(quiz, Is.Null);
        }

        [Test]
        public async Task Test_DeleteQuizByIdDeletesAQuizSuccessfully()
        {
            var result = await this.service.DeleteQuizById(this.testDB.Quiz.Id);
            Assert.That(result, Is.EqualTo(this.testDB.Quiz.Id));

            var quiz = await this.repository.GetByIdAsync<Quiz>(this.testDB.Quiz.Id);
            Assert.That(quiz.IsDeleted, Is.True);
        }

        [Test]
        public async Task Test_DeleteQuizByIdReturnsNullIfQuizDoesNotExist()
        {
            var result = await this.service.DeleteQuizById(0);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_DeleteQuizByIdReturnsNullIfQuizIsAlreadyDeleted()
        {
            var result = await this.service.DeleteQuizById(this.testDB.DeletedQuiz.Id);
            Assert.That(result, Is.Null);
        }

        private CreateQuizViewModel CreateQuizModel()
        {
            return new CreateQuizViewModel()
            {
                Title = "Demo title",
                Description = "Demo description",
                InstantMode = true,
                Questions = new List<CreateQuestionViewModel>()
                { 
                    new CreateQuestionViewModel()
                    {
                        Prompt = "Demo prompt",
                        Type = QuestionTypes.Text,
                        Answers = new List<CreateAnswerViewModel>()
                        {
                            new CreateAnswerViewModel()
                            {
                                Value = "demo answer",
                                Correct = true,
                            }
                        }
                    }
                } 
            };
        }
    }
}
