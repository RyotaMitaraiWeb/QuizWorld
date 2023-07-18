using MockQueryable.Moq;
using Moq;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.Web.Services.GradeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.GradeServiceUnitTests
{
    public class UnitTest
    {
        public Mock<IRepository> repositoryMock;
        public GradeService service;
        public GradedQuestionViewModel question;
       

        [SetUp]
        public void Setup()
        {
            this.repositoryMock = new Mock<IRepository>();
            this.service = new GradeService(this.repositoryMock.Object);
            this.question = new GradedQuestionViewModel()
            {
                Answers = new List<AnswerViewModel>()
                {
                    new AnswerViewModel()
                    {
                        Id = "1",
                        Value = "a",
                    }
                },
                Id = "1",
                InstantMode = true,
            };
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionByIdReturnsAGradedQuestionWithStringId()
        {
            string id = Guid.NewGuid().ToString();
            this.repositoryMock
                .Setup(r => r.AllReadonly<Question>())
                .Returns(this.generateMockQuestions(true, Guid.Parse(id)));

            var result = await this.service.GetCorrectAnswersForQuestionById(id, 1);
            Assert.Multiple(() =>
            {
                Assert.That(result.Answers.Count(), Is.EqualTo(1));
                Assert.That(result.Answers.First().Id.ToString(), Is.EqualTo(id));
            });
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionByIdReturnsAGradedQuestionWithGuid()
        {
            Guid id = Guid.NewGuid();
            this.repositoryMock
                .Setup(r => r.AllReadonly<Question>())
                .Returns(this.generateMockQuestions(true, id));

            var result = await this.service.GetCorrectAnswersForQuestionById(id, 1);
            Assert.Multiple(() =>
            {
                Assert.That(result.Answers.Count(), Is.EqualTo(1));
                Assert.That(result.Answers.First().Id.ToString(), Is.EqualTo(id.ToString()));
            });
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionByIdThrowsIfQuizIsNotInInstantMode()
        {
            Guid id = Guid.NewGuid();
            this.repositoryMock
                .Setup(r => r.AllReadonly<Question>())
                .Returns(this.generateMockQuestions(false, id));

            try
            {
                var result = await this.service.GetCorrectAnswersForQuestionById(id, 1);
                Assert.Fail("Method should have thrown but passed");
            }
            catch (InvalidOperationException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail("Method should have thrown but passed or threw a different error. Error message: " + e.Message);
            }
            
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionByIdReturnsNullIfItCannotRetrieveAQuestion()
        {
            Guid id = Guid.NewGuid();
            this.repositoryMock
                .Setup(r => r.AllReadonly<Question>())
                .Returns(new List<Question>().BuildMock());

            var result = await this.service.GetCorrectAnswersForQuestionById(id, 1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionsByQuizIdReturnsAListOfGradedQuestions()
        {
            Guid id = Guid.NewGuid();
            this.repositoryMock
                .Setup(r => r.AllReadonly<Question>())
                .Returns(this.generateMockQuestions(false, id));

            var result = await this.service.GetCorrectAnswersForQuestionsByQuizId(1, 1);
            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(id.ToString()));
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionsByQuizIdReturnsNullIfItCannotRetrieveAQuestion()
        {
            Guid id = Guid.NewGuid();
            this.repositoryMock
                .Setup(r => r.AllReadonly<Question>())
                .Returns(new List<Question>().BuildMock());

            var result = await this.service.GetCorrectAnswersForQuestionsByQuizId(1, 1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionsByQuizIdThrowsIfQuizIsInInstantMode()
        {
            Guid id = Guid.NewGuid();
            this.repositoryMock
                .Setup(r => r.AllReadonly<Question>())
                .Returns(this.generateMockQuestions(true, id));

            try
            {
                var result = await this.service.GetCorrectAnswersForQuestionsByQuizId(1, 1);
                Assert.Fail("Method should have thrown but passed");
            }
            catch (InvalidOperationException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail("Method should have thrown but passed or threw a different error. Error message: " + e.Message);
            }

        }

        private IQueryable<Question> generateMockQuestions(bool instantMode, Guid questionId)
        {
            var questions = new List<Question>()
            {
                new Question()
                {
                    Id = questionId,
                    Prompt = "1",
                    QuestionTypeId = 1,
                    Version = 1,
                    Quiz = new Quiz()
                    {
                        InstantMode = instantMode,
                    },
                    QuizId = 1,
                    Answers = new List<Answer>()
                    {
                        new Answer()
                        {
                            Id = Guid.NewGuid(),
                            Value = "a",
                            Correct = true,
                        },
                        new Answer()
                        {
                            Id = Guid.NewGuid(),
                            Value = "b",
                            Correct = false,
                        }
                    }
                },
                new Question()
                {
                    Id = Guid.NewGuid(),
                    Prompt = "1",
                    QuestionTypeId = 1,
                    Version = 2,
                    Quiz = new Quiz()
                    {
                        InstantMode = instantMode,
                    },
                    QuizId = 1,
                    Answers = new List<Answer>()
                    {
                        new Answer()
                        {
                            Id = Guid.NewGuid(),
                            Value = "a",
                            Correct = true,
                        },
                        new Answer()
                        {
                            Id = Guid.NewGuid(),
                            Value = "b",
                            Correct = false,
                        }
                    }
                }
            };

            return questions.BuildMock();
        }
    }
}
