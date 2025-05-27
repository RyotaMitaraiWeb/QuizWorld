using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using QuizWorld.Web.Contracts.Quiz;
using QuizWorld.Web.Controllers;
using static QuizWorld.Common.Errors.QuizError;

namespace Tests.Unit.Controllers
{
    public class QuizControllerTests
    {
        public QuizController QuizController { get; set; }
        public IQuizService QuizService { get; set; }

        [SetUp]
        public void Setup()
        {
            QuizService = Substitute.For<IQuizService>();
            QuizController = new QuizController(QuizService);
        }

        [Test]
        [TestCase(QuizGetError.IsDeleted)]
        [TestCase(QuizGetError.DoesNotExist)]
        public async Task Test_GetByIdReturnsNotFoundForErrorResults(QuizGetError error)
        {
            int id = 1;
            QuizService.GetAsync(id).Returns(
                Result<QuizViewModel, QuizGetError>.Failure(error));

            var result = await QuizController.GetById(id);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Test_GetByIdReturnsOkWhenSuccessful()
        {
            int id = 1;
            QuizService.GetAsync(id).Returns(
                Result<QuizViewModel, QuizGetError>.Success(new QuizViewModel()
                {
                    Id = id,
                }));

            var result = await QuizController.GetById(id);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}
