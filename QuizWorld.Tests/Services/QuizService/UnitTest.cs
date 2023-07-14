using Microsoft.Identity.Client;
using MockQueryable.Moq;
using Moq;
using QuizWorld.Common.Constants.Sorting;
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
        public Guid randomGuid = Guid.NewGuid();

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

        [Test]
        public async Task Test_DeleteQuizByIdReturnsTheIdOfTheQuizIfSuccessful()
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
                .Setup(r => r.GetByIdAsync<Quiz>(1))
                .ReturnsAsync(testQuiz.First());

            var result = await this.service.DeleteQuizById(1);
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task Test_DeleteQuizByIdReturnsNullIfItCannotFindAQuiz()
        {
            var quiz = new List<Quiz>();
            this.repositoryMock
                .Setup(r => r.GetByIdAsync<Quiz>(1))
                .ReturnsAsync(() => null);

            var result = await this.service.DeleteQuizById(1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_DeleteQuizByIdReturnsNullIfQuizHasAlreadyBeenDeleted()
        {
            var testQuiz = new Quiz()
            {
                Id = 1,
                Title = "some title",
                Description = "some description",
                InstantMode = true,
                Version = 1,
                IsDeleted = true,
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

            };

            this.repositoryMock
                .Setup(r => r.GetByIdAsync<Quiz>(1))
                .ReturnsAsync(testQuiz);

            var result = await this.service.DeleteQuizById(1);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_EditQuizByIdReturnsTheIdOfTheQuizIfSuccessful()
        {
            var testQuiz = new Quiz()
            {
                Id = 1,
                Title = "some title",
                Description = "some description",
                InstantMode = true,
                Version = 1,
                IsDeleted = false,
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

            };

            var quiz = new EditQuizViewModel()
            {
                Title = "some title",
                Description = "some description",
                Questions = this.generateQuestions(4, 2)
            };

            this.repositoryMock
                .Setup(r => r.GetByIdAsync<Quiz>(1))
                .ReturnsAsync(testQuiz);

            var result = await this.service.EditQuizById(1, quiz);
            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public async Task Test_EditQuizByIdReturnsNullIfQuizIsDeleted()
        {
            var testQuiz = new Quiz()
            {
                Id = 1,
                Title = "some title",
                Description = "some description",
                InstantMode = true,
                Version = 1,
                IsDeleted = true,
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

            };

            var quiz = new EditQuizViewModel()
            {
                Title = "some title",
                Description = "some description",
                Questions = this.generateQuestions(4, 2)
            };

            this.repositoryMock
                .Setup(r => r.GetByIdAsync<Quiz>(1))
                .ReturnsAsync(testQuiz);

            var result = await this.service.EditQuizById(1, quiz);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_EditQuizByIdReturnsNullIfQuizDoesNotExist()
        {
            var quiz = new EditQuizViewModel()
            {
                Title = "some title",
                Description = "some description",
                Questions = this.generateQuestions(4, 2)
            };

            this.repositoryMock
                .Setup(r => r.GetByIdAsync<Quiz>(1))
                .ReturnsAsync(() => null);

            var result = await this.service.EditQuizById(1, quiz);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_GetAllQuizzesReturnsAListOfQuizzesAndCorrectTotalCount()
        {
            var date = DateTime.Now;
            var list = new List<Quiz>()
            {
                new Quiz()
                {
                    Title = "a",
                    CreatedOn = date.AddDays(1),
                    UpdatedOn = date.AddDays(2),
                    NormalizedTitle = "A",
                },
                new Quiz()
                {
                    Title = "c",
                    CreatedOn = date.AddDays(3),
                    UpdatedOn = date.AddDays(3),
                    NormalizedTitle = "C",
                },
                new Quiz()
                {
                    Title = "b",
                    CreatedOn = date,
                    UpdatedOn = date,
                    NormalizedTitle = "B",
                },
                new Quiz()
                {
                    Title = "d",
                    CreatedOn = date.AddDays(-1),
                    UpdatedOn = date,
                    NormalizedTitle = "D",
                },
            };

            var mockList = list.BuildMock();

            this.repositoryMock
                .Setup(r => r.AllReadonly<Quiz>())
                .Returns(mockList);

            var result = await this.service.GetAllQuizzes(2, SortingCategories.Title, SortingOrders.Descending, 2);
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(4));
                var quizzes = result.Quizzes.ToArray();

                Assert.That(quizzes, Has.Length.EqualTo(2));
                var quiz1 = quizzes[0];
                var quiz2 = quizzes[1];

                Assert.That(quiz1.Title, Is.EqualTo("b"));
                Assert.That(quiz2.Title, Is.EqualTo("a"));
            });
        }

        [Test]
        [TestCase("00000000-0000-0000-0000-000000000000")]
        public async Task Test_GetUserQuizzesReturnsAListOfQuizzesAndCorrectTotalCountWithStringId(string query)
        {
            var date = DateTime.Now;
            var list = new List<Quiz>()
            {
                new Quiz()
                {
                    CreatorId = Guid.Parse(query),
                    Title = "abcd",
                    CreatedOn = date.AddDays(1),
                    UpdatedOn = date.AddDays(2),
                    NormalizedTitle = "ABCD",
                },
                new Quiz()
                {
                    CreatorId = Guid.Parse(query),
                    Title = "acd",
                    CreatedOn = date.AddDays(3),
                    UpdatedOn = date.AddDays(3),
                    NormalizedTitle = "ACD",
                },
                new Quiz()
                {
                    CreatorId = Guid.Parse(query),
                    Title = "ADC",
                    CreatedOn = date,
                    UpdatedOn = date,
                    NormalizedTitle = "ADC",
                },
                new Quiz()
                {
                    Title = "d",
                    CreatedOn = date.AddDays(-1),
                    UpdatedOn = date,
                    NormalizedTitle = "D",
                },
            };

            var mockList = list.BuildMock();

            this.repositoryMock
                .Setup(r => r.AllReadonly<Quiz>())
                .Returns(mockList);

            var result = await this.service.GetUserQuizzes(query, 2, SortingCategories.Title, SortingOrders.Descending, 2);
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(4));
                var quizzes = result.Quizzes.ToArray();

                Assert.That(quizzes, Has.Length.EqualTo(2));
                var quiz1 = quizzes[0];
                var quiz2 = quizzes[1];

                Assert.That(quiz1.Title, Is.EqualTo("acd"));
                Assert.That(quiz2.Title, Is.EqualTo("abcd"));
            });
        }

        [Test]
        public async Task Test_GetUserQuizzesReturnsAListOfQuizzesAndCorrectTotalCountWithGuid()
        {
            Guid query = Guid.NewGuid();
            var date = DateTime.Now;
            var list = new List<Quiz>()
            {
                new Quiz()
                {
                    CreatorId = query,
                    Title = "abcd",
                    CreatedOn = date.AddDays(1),
                    UpdatedOn = date.AddDays(2),
                    NormalizedTitle = "ABCD",
                },
                new Quiz()
                {
                    CreatorId = query,
                    Title = "acd",
                    CreatedOn = date.AddDays(3),
                    UpdatedOn = date.AddDays(3),
                    NormalizedTitle = "ACD",
                },
                new Quiz()
                {
                    CreatorId = query,
                    Title = "ADC",
                    CreatedOn = date,
                    UpdatedOn = date,
                    NormalizedTitle = "ADC",
                },
                new Quiz()
                {
                    CreatorId = query,
                    Title = "ABAC",
                    CreatedOn = date,
                    UpdatedOn = date,
                    NormalizedTitle = "ABAC",
                },

                new Quiz()
                {
                    Title = "d",
                    CreatedOn = date.AddDays(-1),
                    UpdatedOn = date,
                    NormalizedTitle = "D",
                },
            };

            var mockList = list.BuildMock();

            this.repositoryMock
                .Setup(r => r.AllReadonly<Quiz>())
                .Returns(mockList);

            var result = await this.service.GetUserQuizzes(query, 2, SortingCategories.Title, SortingOrders.Descending, 2);
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(4));
                var quizzes = result.Quizzes.ToArray();

                Assert.That(quizzes, Has.Length.EqualTo(2));
                var quiz1 = quizzes[0];
                var quiz2 = quizzes[1];

                Assert.That(quiz1.Title, Is.EqualTo("abcd"));
                Assert.That(quiz2.Title, Is.EqualTo("ABAC"));
            });
        }

        [Test]
        [TestCase("a")]
        [TestCase("A")]
        public async Task Test_GetQuizzesByQueryReturnsAListOfQuizzesAndCorrectTotalCount(string query)
        {
            var date = DateTime.Now;
            var list = new List<Quiz>()
            {
                new Quiz()
                {
                    Title = "aA",
                    CreatedOn = date.AddDays(1),
                    UpdatedOn = date.AddDays(2),
                    NormalizedTitle = "AA",
                },
                new Quiz()
                {
                    Title = "AAA",
                    CreatedOn = date.AddDays(3),
                    UpdatedOn = date.AddDays(3),
                    NormalizedTitle = "AAA",
                },
                new Quiz()
                {
                    Title = "a",
                    CreatedOn = date,
                    UpdatedOn = date,
                    NormalizedTitle = "A",
                },
                new Quiz()
                {
                    Title = "d",
                    CreatedOn = date.AddDays(-1),
                    UpdatedOn = date,
                    NormalizedTitle = "D",
                },
            };

            var mockList = list.BuildMock();

            this.repositoryMock
                .Setup(r => r.AllReadonly<Quiz>())
                .Returns(mockList);

            var result = await this.service.GetQuizzesByQuery(query, 2, SortingCategories.Title, SortingOrders.Descending, 2);
            Assert.Multiple(() =>
            {
                Assert.That(result.Total, Is.EqualTo(3));
                var quizzes = result.Quizzes.ToArray();

                Assert.That(quizzes, Has.Length.EqualTo(1));
                var quiz1 = quizzes[0];
                

                Assert.That(quiz1.Title, Is.EqualTo("a"));
            });
        }
    }
}
