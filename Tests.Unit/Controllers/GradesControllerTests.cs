using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using QuizWorld.Common.Result;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.ViewModels.Question;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Controllers;
using static QuizWorld.Common.Errors.GradeError;

namespace Tests.Unit.Controllers
{
    public class GradesControllerTests
    {
        public IGradeService GradeService { get; set; }
        public GradesController GradesController { get; set; }

        [SetUp]
        public void Setup()
        {
            GradeService = Substitute.For<IGradeService>();
            GradesController = new GradesController(GradeService);
        }

        [Test]
        public async Task Test_GradeQuestionReturns200OkForSuccess()
        {
            Guid questionId = Guid.NewGuid();
            int version = 1;

            GradeService
                .GradeQuestion(questionId.ToString(), version)
                .Returns(
                    Result<GradedQuestionViewModel, GradeQuestionError>
                        .Success(new GradedQuestionViewModel())
                    );

            var result = await GradesController.GradeQuestion(questionId.ToString(), version);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [TestCase(GradeQuestionError.IsNotInstantMode, StatusCodes.Status400BadRequest)]
        [TestCase(GradeQuestionError.QuestionOrVersionDoesNotExist, StatusCodes.Status404NotFound)]
        public async Task Test_GradeQuestionReturnsCorrectResponseForErrors(GradeQuestionError error, int statusCode)
        {
            Guid questionId = Guid.NewGuid();
            int version = 1;

            GradeService
                .GradeQuestion(questionId.ToString(), version)
                .Returns(
                    Result<GradedQuestionViewModel, GradeQuestionError>
                        .Failure(error)
                    );

            var result = await GradesController.GradeQuestion(questionId.ToString(), version);

            var response = result as StatusCodeResult;
            Assert.That(response?.StatusCode, Is.EqualTo(statusCode));
        }

        [Test]
        public async Task Test_GradeQuizReturns200OkForSuccess()
        {
            int quizId = 1;
            int version = 1;

            GradeService
                .GradeQuiz(quizId, version)
                .Returns(
                    Result<IList<GradedQuestionViewModel>, GradeEntireQuizError>
                        .Success([])
                    );

            var result = await GradesController.GradeQuiz(quizId, version);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [TestCase(GradeEntireQuizError.IsInstantMode, StatusCodes.Status400BadRequest)]
        [TestCase(GradeEntireQuizError.QuizOrVersionDoesNotExist, StatusCodes.Status404NotFound)]
        public async Task Test_GradeQuizReturnsCorrectResponseForErrors(GradeEntireQuizError error, int statusCode)
        {
            int quizId = 1;
            int version = 1;

            GradeService
                .GradeQuiz(quizId, version)
                .Returns(
                    Result<IList<GradedQuestionViewModel>, GradeEntireQuizError>
                        .Failure(error)
                    );

            var result = await GradesController.GradeQuiz(quizId, version);

            var response = result as StatusCodeResult;
            Assert.That(response?.StatusCode, Is.EqualTo(statusCode));
        }
    }
}
