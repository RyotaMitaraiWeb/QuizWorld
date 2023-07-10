using Moq;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Services.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.QuizServiceUnitTests
{
    public class UnitTest
    {
        public QuizService service;
        public Mock<IRepository> repositoryMock;

        private IEnumerable<CreateAnswerViewModel> generateAnswers(int amount)
        {
            var answers = new List<CreateAnswerViewModel>();
            for (int i = 0; i < amount; i++)
            {
                answers.Add(new CreateAnswerViewModel()
                {
                    Correct = true,
                    Value = "a",
                });
            }

            return answers;
        }

        private IEnumerable<CreateQuestionViewModel> generateQuestions(int amount, int answersAmount)
        {
            var answers = this.generateAnswers(answersAmount);
            var questions = new List<CreateQuestionViewModel>();
            for (int i = 0; i < amount; i++)
            {
                questions.Add(new CreateQuestionViewModel()
                {
                    Prompt = "some prompt",
                    Type = QuestionTypes.Text,
                    Answers = answers,
                });
            }

            return questions;
        }

        [SetUp]
        public void Setup()
        {
            this.repositoryMock = new Mock<IRepository>();
            this.service = new QuizService(this.repositoryMock.Object);
        }

        [Test]
        public async Task Test_CreateQuizReturnsTheIdOfTheQuizIfSuccessful()
        {
            var quiz = new CreateQuizViewModel()
            {
                Title = "some title",
                Description = "some description",
                InstantMode = true,
                Questions = this.generateQuestions(3, 2)
            };

            //this.repositoryMock
            //    .Setup(r => r.AddAsync(It.IsAny<Quiz>()))
            //    .Callback

            var result = await this.service.CreateQuiz(quiz, "a");
            Assert.That(result, Is.EqualTo(1));
        }
    }
}
