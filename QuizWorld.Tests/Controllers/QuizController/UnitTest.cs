using Microsoft.AspNetCore.Mvc;
using Moq;
using QuizWorld.Common.Constants.Types;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.Common;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.JsonWebToken;
using QuizWorld.Web.Contracts.Quiz;
using QuizWorld.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Controllers.QuizControllerUnitTests
{
    public class UnitTest
    {
        public QuizController controller;
        public Mock<IJwtService> jwtServiceMock;
        public Mock<IQuizService> quizServiceMock;
        public UserViewModel user;

        public CreateQuizViewModel createQuizViewModel;
        public EditQuizViewModel editQuizViewModel;

        [SetUp]
        public void Setup()
        {
            this.user = new UserViewModel()
            {
                Id = Guid.NewGuid().ToString(),
                Username = "ryota1",
                Roles = new string[] { "User" }
            };

            this.quizServiceMock = new Mock<IQuizService>();
            this.jwtServiceMock = new Mock<IJwtService>();

            this.controller = new QuizController(this.quizServiceMock.Object, this.jwtServiceMock.Object);
            this.createQuizViewModel = new CreateQuizViewModel()
            {
                Title = "some quiz",
                Description = "some quiz description",
                InstantMode = true,
                Questions = new List<CreateQuestionViewModel>()
                {
                    new CreateQuestionViewModel()
                    {
                        Type = QuestionTypes.Text,
                        Prompt = "what is 1 + 1?",
                        Answers = new List<CreateAnswerViewModel>()
                        {
                            new CreateAnswerViewModel()
                            {
                                Value = "2",
                                Correct = true,
                            },
                        },
                    },
                },
            };

            this.editQuizViewModel = new EditQuizViewModel()
            {
                Title = "new title quiz",
                Description = "new description quiz",
                Questions = this.createQuizViewModel.Questions,
            };
        }

        [Test]
        public async Task Test_CreateReturns201WithTheReturnedIdIfTheQuizIsSuccessfullyCreated()
        {
            this.jwtServiceMock
                .Setup(js => js.RemoveBearer("a"))
                .Returns("a");

            this.jwtServiceMock
                .Setup(js => js.DecodeJWT("a"))
                .Returns(this.user);

            this.quizServiceMock
                .Setup(qs => qs.CreateQuiz(this.createQuizViewModel, user.Id))
                .ReturnsAsync(1);

            var response = await this.controller.Create(this.createQuizViewModel, "a") as CreatedResult;
            var value = response.Value as CreatedResponseViewModel;
            Assert.That(value.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task Test_CreateReturnsServiceUnavailableIfQuizServiceThrowsAGenericException()
        {
            this.jwtServiceMock
                .Setup(js => js.RemoveBearer("a"))
                .Returns("a");

            this.jwtServiceMock
                .Setup(js => js.DecodeJWT("a"))
                .Returns(this.user);

            this.quizServiceMock
                .Setup(qs => qs.CreateQuiz(this.createQuizViewModel, user.Id))
                .ThrowsAsync(new Exception());

            var response = await this.controller.Create(this.createQuizViewModel, "a");
            
            Assert.That(response, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task Test_CreateReturnsUnauthorizedIfQuizServiceThrowsAnInvalidOperationException()
        {
            this.jwtServiceMock
                .Setup(js => js.RemoveBearer("a"))
                .Returns("a");

            this.jwtServiceMock
                .Setup(js => js.DecodeJWT("a"))
                .Returns(this.user);

            this.quizServiceMock
                .Setup(qs => qs.CreateQuiz(this.createQuizViewModel, user.Id))
                .ThrowsAsync(new InvalidOperationException());

            var response = await this.controller.Create(this.createQuizViewModel, "a");

            Assert.That(response, Is.TypeOf<UnauthorizedResult>());
        }

        [Test]
        public async Task Test_GetReturnsOkWithTheReturnedQuiz()
        {
            this.quizServiceMock
                .Setup(qs => qs.GetQuizById(1))
                .ReturnsAsync(new QuizViewModel { Id = 1, Title = "test" });


            var response = await this.controller.Get(1) as OkObjectResult;
            var value = response.Value as QuizViewModel;

            Assert.Multiple(() =>
            {
                Assert.That(value.Title, Is.EqualTo("test"));
                Assert.That(value.Id, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task Test_GetReturnsNotFoundIfGetQuizReturnsNull()
        {
            this.quizServiceMock
                .Setup(qs => qs.GetQuizById(1))
                .ReturnsAsync(() => null);


            var response = await this.controller.Get(1);
            Assert.That(response, Is.TypeOf<NotFoundResult>());
            
        }

        [Test]
        public async Task Test_GetReturnsServiceUnavailableIfGetQuizThrows()
        {
            this.quizServiceMock
                .Setup(qs => qs.GetQuizById(1))
                .ThrowsAsync(new Exception());


            var response = await this.controller.Get(1);
            Assert.That(response, Is.TypeOf<StatusCodeResult>());

        }

        [Test]
        public async Task Test_DeleteReturnsNoContentIfDeleteQuizByIdReturnsAnId()
        {
            this.quizServiceMock
                .Setup(qs => qs.DeleteQuizById(1))
                .ReturnsAsync(1);


            var response = await this.controller.Delete(1);
            Assert.That(response, Is.TypeOf<NoContentResult>());

        }

        [Test]
        public async Task Test_DeleteReturnsNotFoundIfDeleteQuizByIdReturnsNull()
        {
            this.quizServiceMock
                .Setup(qs => qs.DeleteQuizById(1))
                .ReturnsAsync(() => null);


            var response = await this.controller.Delete(1);
            Assert.That(response, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Test_DeleteReturnsServiceUnavailableIfDeleteQuizByIdThrows()
        {
            this.quizServiceMock
                .Setup(qs => qs.DeleteQuizById(1))
                .ThrowsAsync(new Exception());


            var response = await this.controller.Delete(1);
            Assert.That(response, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task Test_EditReturnsNoContentIfEditQuizByIdReturnsAnId()
        {
            this.quizServiceMock
                .Setup(qs => qs.EditQuizById(1, this.editQuizViewModel))
                .ReturnsAsync(1);

            var response = await this.controller.Edit(this.editQuizViewModel, 1);
            Assert.That(response, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Test_EditReturnsNotFoundIfEditQuizByIdReturnsNull()
        {
            this.quizServiceMock
                .Setup(qs => qs.EditQuizById(1, this.editQuizViewModel))
                .ReturnsAsync(() => null);

            var response = await this.controller.Edit(this.editQuizViewModel, 1);
            Assert.That(response, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Test_EditReturnsServiceUnavailableIfEditQuizByIdThrows()
        {
            this.quizServiceMock
                .Setup(qs => qs.EditQuizById(1, this.editQuizViewModel))
                .ThrowsAsync(new Exception());

            var response = await this.controller.Edit(this.editQuizViewModel, 1);
            Assert.That(response, Is.TypeOf<StatusCodeResult>());
        }
    }
}
