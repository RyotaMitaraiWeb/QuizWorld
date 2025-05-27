using MockQueryable;
using NSubstitute;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Web.Services;
using System.Linq.Expressions;
using Tests.Util;
using static QuizWorld.Common.Errors.QuizError;

namespace Tests.Unit.Services
{
    public class QuizServiceTests
    {
        public IRepository Repository { get; set; }
        public QuizService QuizService { get; set; }
        public List<Quiz> SampleQuizzes { get; set; }

        [SetUp]
        public void Setup()
        {
            Repository = Substitute.For<IRepository>();
            SampleQuizzes = [
                    SampleQuiz.GenerateQuiz(version: 2, instantMode: true, isDeleted: false),
                    SampleQuiz.GenerateQuiz(version: 2, instantMode: true, isDeleted: true),
                    SampleQuiz.GenerateQuiz(version: 2, instantMode: false, isDeleted: false),
                    SampleQuiz.GenerateQuiz(version: 2, instantMode: false, isDeleted: true),
                ];

            QuizService = new QuizService(Repository);
        }

        [Test]
        public async Task Test_GetAsyncReturnsNotFoundErrorIfTheQuizDoesNotExist()
        {
            List<Quiz> list = [];
            var queryable = list.BuildMock();
            Repository.AllReadonly(Arg.Any<Expression<Func<Quiz, bool>>>())
                .Returns(queryable);

            var result = await QuizService.GetAsync(1);
            Assert.That(result.Error, Is.EqualTo(QuizGetError.DoesNotExist));
        }

        [Test]
        public async Task Test_GetAsyncReturnsDeletedErrorIfTheQuizDoesNotExist()
        {
            List<Quiz> list = [SampleQuizzes[1]];
            
            Repository.AllReadonly(Arg.Any<Expression<Func<Quiz, bool>>>())
                .Returns(list.BuildMock());

            var result = await QuizService.GetAsync(1);
            Assert.That(result.Error, Is.EqualTo(QuizGetError.IsDeleted));
        }

        [Test]
        public async Task Test_GetAsyncReturnsTheQuizAndWithOnlyTheQuestionsThatMatchItsVersion()
        {
            Quiz sampleQuiz = SampleQuizzes.First(quiz => !quiz.IsDeleted);
            int id = sampleQuiz.Id;

            Repository
                .AllReadonly(Arg.Any<Expression<Func<Quiz, bool>>>())
                .Returns(SampleQuizzes.BuildMock());

            var result = await QuizService.GetAsync(id);

            Assert.Multiple(() =>
            {
                Assert.That(result.Value.Questions.Count(), Is.EqualTo(3));
                Assert.That(result.Value.Id, Is.EqualTo(id));
            });
        }

        [TearDown]
        public void Teardown()
        {
            Repository.Dispose();
        }
    }
}
