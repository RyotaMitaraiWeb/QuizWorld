using Microsoft.AspNetCore.Mvc;
using Moq;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.Web.Contracts.Quiz;
using QuizWorld.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Controllers.GradeControllerUnitTests
{
    public class UnitTest
    {
        public Mock<IGradeServiceDeprecated> gradeServiceMock;
        public GradeControllerV1 controller;
        public GradedQuestionViewModel question;

        [SetUp]
        public void Setup()
        {
            this.gradeServiceMock = new Mock<IGradeServiceDeprecated>();
            this.controller = new GradeControllerV1(this.gradeServiceMock.Object);
            this.question = new GradedQuestionViewModel()
            {
                Id = "a",
                Answers = new List<AnswerViewModel>()
                {
                    new AnswerViewModel()
                    {
                        Value = "a",
                        Id = "b",
                    }
                }
            };
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionReturnsOkWithTheGradedQuestionFromGradeService()
        {
            string id = Guid.NewGuid().ToString();
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionById(id, 1))
                .ReturnsAsync(this.question);

            var result = await this.controller.GetCorrectAnswersForQuestion(id, 1) as OkObjectResult;
            var value = result.Value as GradedQuestionViewModel;

            Assert.That(value, Is.EqualTo(this.question));
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionReturnsNotFoundIfGradeServiceReturnsNull()
        {
            string id = Guid.NewGuid().ToString();
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionById(id, 1))
                .ReturnsAsync(() => null);

            var result = await this.controller.GetCorrectAnswersForQuestion(id, 1);
            
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionReturnsBadRequestIfGradeServiceThrowsAnInvalidOperationException()
        {
            string id = Guid.NewGuid().ToString();
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionById(id, 1))
                .ThrowsAsync(new InvalidOperationException());

            var result = await this.controller.GetCorrectAnswersForQuestion(id, 1);

            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionReturnsServiceUnavailableIfGradeServiceThrowsAGenericException()
        {
            string id = Guid.NewGuid().ToString();
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionById(id, 1))
                .ThrowsAsync(new Exception());

            var result = await this.controller.GetCorrectAnswersForQuestion(id, 1);

            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuizReturnsOkWithTheGradedQuestionsFromGradeService()
        {
            var list = new List<GradedQuestionViewModel> { this.question };
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionsByQuizId(1, 1))
                .ReturnsAsync(list);

            var result = await this.controller.GetCorrectAnswersForQuiz(1, 1) as OkObjectResult;
            var value = result.Value as List<GradedQuestionViewModel>;

            Assert.That(value, Is.EqualTo(list));
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuizReturnsBadRequestIfGradeServiceThrowsAnInvalidOperationException()
        {
            var list = new List<GradedQuestionViewModel> { this.question };
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionsByQuizId(1, 1))
                .ThrowsAsync(new InvalidOperationException());

            var result = await this.controller.GetCorrectAnswersForQuiz(1, 1);
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuizReturnsServiceUnavailableIfGradeServiceThrowsAGenericException()
        {
            var list = new List<GradedQuestionViewModel> { this.question };
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionsByQuizId(1, 1))
                .ThrowsAsync(new Exception());

            var result = await this.controller.GetCorrectAnswersForQuiz(1, 1);
            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuizReturnsNotFoundIfGradeServiceReturnsNull()
        {
            var list = new List<GradedQuestionViewModel> { this.question };
            this.gradeServiceMock
                .Setup(gs => gs.GetCorrectAnswersForQuestionsByQuizId(1, 1))
                .ReturnsAsync(() => null);

            var result = await this.controller.GetCorrectAnswersForQuiz(1, 1);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
