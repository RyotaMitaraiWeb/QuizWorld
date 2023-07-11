﻿using Microsoft.Identity.Client;
using MockQueryable.Moq;
using Moq;
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
using System.Linq.Expressions;
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
        public async Task Test_CreateQuizReturnsTheIdOfTheQuizWhenPassedAStringIdAndIsSuccessful()
        {
            var quiz = new CreateQuizViewModel()
            {
                Title = "some title",
                Description = "some description",
                InstantMode = true,
                Questions = this.generateQuestions(3, 2)
            };

            var result = await this.service.CreateQuiz(quiz, new Guid());
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task Test_CreateQuizReturnsTheIdOfTheQuizWhenPassedAGuidAndIsSuccessful()
        {
            var quiz = new CreateQuizViewModel()
            {
                Title = "some title",
                Description = "some description",
                InstantMode = true,
                Questions = this.generateQuestions(3, 2)
            };

            var result = await this.service.CreateQuiz(quiz, new Guid());
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task Test_GetQuizByIdReturnsAQuizViewModelOfTheEntityFound()
        {
            var testQuiz = new List<Quiz>()
            {
                new Quiz()
                {
                    Id = 1,
                    Title = "some title",
                    Description = "some description",
                    InstantMode = true,
                    Version = 1,
                    Questions = new List<Question>()
                    {
                        new Question()
                        {
                            Prompt = "a",
                            Id = Guid.NewGuid(),
                            Version = 1,
                            Answers = new List<Answer>()
                            {
                                new Answer()
                                {
                                    Value = "a",
                                    Correct = true,
                                }
                            }
                        }
                    }
                }
            };

            var mockQuiz = testQuiz.BuildMock();
            
            this.repositoryMock
                .Setup(r => r.AllReadonly(It.IsAny<Expression<Func<Quiz, bool>>>()))
                .Returns(mockQuiz);
                    
            var result = await this.service.GetQuizById(1);
            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(1));
                Assert.That(result.Title, Is.EqualTo("some title"));
                Assert.That(result.Description, Is.EqualTo("some description"));
                Assert.That(result.InstantMode, Is.True);
                Assert.That(result.Version, Is.EqualTo(1));

            });
        }

        [Test]
        public async Task Test_GetQuizByIdReturnsNullIfItCannotFindAQuiz()
        {
            var quiz = new List<Quiz>();
            var mockQuiz = quiz.BuildMock<Quiz>();
            this.repositoryMock
                .Setup(r => r.AllReadonly(It.IsAny<Expression<Func<Quiz, bool>>>()))
                .Returns(mockQuiz);

            var result = await this.service.GetQuizById(1);
            Assert.That(result, Is.Null);
        }
    }
}
