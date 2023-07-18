using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.Web.Services.GradeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.GradeServiceInMemoryTests
{
    public class InMemory
    {
        public GradeService service;
        public TestDB testDb;
        public Repository repository;

        [SetUp]
        public void Setup()
        {
            this.testDb = new TestDB();
            this.repository = this.testDb.repository;
            this.service = new GradeService(this.repository);
        }

        [Test]
        [TestCase(1, 1, 1)]
        [TestCase(2, 3, 2)]
        public async Task Test_GetCorrectAnswersForQuestionByIdReturnsTheCorrectAnswersOfTheQuestionForTheGivenVersion(int version, int expectedAmountOfAnswers, int questionType)
        {
            var quiz = this.testDb.Quiz;
            var question = quiz.Questions.First(q => q.Version == version && q.QuestionTypeId == questionType);

            var result = await this.service.GetCorrectAnswersForQuestionById(question.Id, version);
            Assert.That(result.Answers.Count(), Is.EqualTo(expectedAmountOfAnswers));
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionByIdReturnsNullIfItCannotRetrieveAQuestionOrQuiz()
        {
            var resultForNonExistantQuestion = await this.service.GetCorrectAnswersForQuestionById(new Guid(), 1);
            Assert.That(resultForNonExistantQuestion, Is.Null);

            var resultForDeletedQuiz = await this.service.GetCorrectAnswersForQuestionById(this.testDb.DeletedQuiz.Questions.First().Id, 1);
            Assert.That(resultForDeletedQuiz, Is.Null);

            var resultForNonexistantVersion = await this.service.GetCorrectAnswersForQuestionById(this.testDb.Quiz.Questions.First().Id, 0);
            Assert.That(resultForNonexistantVersion, Is.Null);
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionByIdThrowsInvalidOperationExceptionIfTheQuizIsNotInInstantMode()
        {
            var quiz = this.testDb.NonInstantQuiz;
            var question = quiz.Questions.First();

            try
            {
                var result = await this.service.GetCorrectAnswersForQuestionById(question.Id, question.Version);
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
        [TestCase(1, 1)]
        [TestCase(2, 3)]
        public async Task Test_GetCorrectAnswersForQuestionsByQuizIdReturnsAListOfGradedQuestionsForTheRespectiveVersion(int version, int expectedAmountOfQuestions)
        {
            var quiz = this.testDb.NonInstantQuiz;

            var result = await this.service.GetCorrectAnswersForQuestionsByQuizId(quiz.Id, version);

            Assert.That(result.Count(), Is.EqualTo(expectedAmountOfQuestions));
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionsByQuizIdReturnsNullIfItCannotRetrieveAQuiz()
        {
            var resultForNonExistantQuiz = await this.service.GetCorrectAnswersForQuestionsByQuizId(0, 1);
            Assert.That(resultForNonExistantQuiz, Is.Null);

            var resultForDeletedQuiz = await this.service.GetCorrectAnswersForQuestionsByQuizId(this.testDb.DeletedQuiz.Id, 1);
            Assert.That(resultForDeletedQuiz, Is.Null);

            var resultForNonexistantVersion = await this.service.GetCorrectAnswersForQuestionsByQuizId(this.testDb.Quiz.Id, 0);
            Assert.That(resultForNonexistantVersion, Is.Null);
        }

        [Test]
        public async Task Test_GetCorrectAnswersForQuestionsByQuizIdThrowsInvalidOperationExceptionIfTheQuizIsInInstantMode()
        {
            var quiz = this.testDb.Quiz;
            

            try
            {
                var result = await this.service.GetCorrectAnswersForQuestionsByQuizId(quiz.Id, 2);
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
    }
}
