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
            Assert.That(result, Is.EqualTo(2));

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
            Assert.That(result, Is.EqualTo(2));

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
