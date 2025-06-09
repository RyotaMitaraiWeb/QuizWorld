using MockQueryable;
using NSubstitute;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Web.Services;
using System.Linq.Expressions;
using Tests.Util;
using static QuizWorld.Common.Errors.GradeError;

namespace Tests.Unit.Services
{
    public class GradeServiceTests
    {
        public IRepository Repository { get; set; }
        public GradeService GradeService { get; set; }

        [SetUp]
        public void Setup()
        {
            Repository = Substitute.For<IRepository>();
            GradeService = new GradeService(Repository);
        }

        [Test]
        public async Task Test_GradeQuestionReturnsQuestionDoesNotExistErrorWhenOneCannotBeRetrieved()
        {
            var questions = new List<Question>() { SampleQuestions.GenerateQuestion(4, QuestionTypes.Text) };
            var question = questions[0];

            Repository.AllReadonly(Arg.Any<Expression<Func<Question, bool>>>())
                .Returns(new List<Question>().BuildMock());

            var result = await GradeService.GradeQuestion(question.Id.ToString(), 3);

            Assert.That(result.Error, Is.EqualTo(GradeQuestionError.QuestionOrVersionDoesNotExist));
        }

        [Test]
        public async Task Test_GradeQuestionReturnsNotInstantModeErrorWhenNotInInstantMode()
        {
            Guid questionId = Guid.NewGuid();
            var question = new Question()
            {
                Id = questionId,
                Quiz = new Quiz()
                {
                    InstantMode = false,
                    Version = 1,
                },
                Answers = [],
            };

            var questions = new List<Question>() { question };

            Repository
                .AllReadonly(Arg.Any<Expression<Func<Question, bool>>>())
                .Returns(questions.BuildMock());

            var result = await GradeService.GradeQuestion(question.Id.ToString(), question.Version);

            Assert.That(result.Error, Is.EqualTo(GradeQuestionError.IsNotInstantMode));
        }

        [Test]
        public async Task Test_GradeQuestionReturnsSuccessWhenSuccessful()
        {
            Guid questionId = Guid.NewGuid();
            var question = new Question()
            {
                Id = questionId,
                Quiz = new Quiz()
                {
                    InstantMode = true,
                    Version = 1,
                },
                Answers = [],
            };

            var questions = new List<Question>() { question };

            Repository
                .AllReadonly(Arg.Any<Expression<Func<Question, bool>>>())
                .Returns(questions.BuildMock());

            var result = await GradeService.GradeQuestion(question.Id.ToString(), question.Version);

            Assert.That(result.IsSuccess, Is.True);
        }

        [Test]
        public async Task Test_GradeQuizReturnsQuizDoesNotExistErrorWhenOneCannotBeRetrieved()
        {
            Repository.AllReadonly(Arg.Any<Expression<Func<Question, bool>>>())
                .Returns(new List<Question>().BuildMock());

            var result = await GradeService.GradeQuiz(1, 3);

            Assert.That(result.Error, Is.EqualTo(GradeEntireQuizError.QuizOrVersionDoesNotExist));
        }

        [Test]
        public async Task Test_GradeQuizReturnsInstantModeErrorWhenInInstantMode()
        {
            int quizId = 1;
            Guid questionId = Guid.NewGuid();
            var question = new Question()
            {
                Id = questionId,
                Quiz = new Quiz()
                {
                    InstantMode = true,
                    Version = 1,
                    Id = quizId,
                },
                Answers = [],
            };

            var questions = new List<Question>() { question };

            Repository
                .AllReadonly(Arg.Any<Expression<Func<Question, bool>>>())
                .Returns(questions.BuildMock());

            var result = await GradeService.GradeQuiz(quizId, question.Version);

            Assert.That(result.Error, Is.EqualTo(GradeEntireQuizError.IsInstantMode));
        }

        [Test]
        public async Task Test_GradeQuizReturnsSuccessWhenSuccessful()
        {
            int quizId = 1;
            Guid questionId = Guid.NewGuid();
            var question = new Question()
            {
                Id = questionId,
                Quiz = new Quiz()
                {
                    InstantMode = false,
                    Version = 1,
                    Id = quizId,
                },
                Answers = [],
            };

            var questions = new List<Question>() { question };

            Repository
                .AllReadonly(Arg.Any<Expression<Func<Question, bool>>>())
                .Returns(questions.BuildMock());

            var result = await GradeService.GradeQuiz(quizId, question.Version);

            Assert.That(result.IsSuccess, Is.True);
        }

        [TearDown]
        public void Teardown()
        {
            Repository.Dispose();
        }
    }
}
