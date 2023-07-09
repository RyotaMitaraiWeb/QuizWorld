using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Validators.CorrectAndWrongAnswersAmountUnitTest
{
    public class UnitTest
    {
        public CorrectAndWrongAnswersAmount validator;
        private IEnumerable<CreateAnswerViewModel> generateAnswers(int correctAmount, int wrongAmount)
        {
            var answers = new List<CreateAnswerViewModel>();

            for (int i = 0; i < correctAmount; i++)
            {
                answers.Add(new CreateAnswerViewModel()
                {
                    Value = "a",
                    Correct = true,
                });
            }

            for (int i = 0; i < wrongAmount; i++)
            {
                answers.Add(new CreateAnswerViewModel()
                {
                    Value = "a",
                    Correct = false,
                });
            }
            return answers;
        }

        private CreateQuestionViewModel generateQuestion(string questionType, int correctAmount, int wrongAmount)
        {
            var answers = this.generateAnswers(correctAmount, wrongAmount);
            return new CreateQuestionViewModel()
            {
                Prompt = "valid prompt",
                Answers = answers,
                Type = questionType
            };
        }

        [SetUp]
        public void Setup()
        {
            this.validator = new CorrectAndWrongAnswersAmount();
        }

        [Test]
        [TestCase("single", 2, 1)]
        [TestCase("single", 0, 1)]
        [TestCase("multi", 0, 1)]
        [TestCase("text", 0, 0)]
        public void Test_CorrectAndWrongAnswersAmountConsidersItAnErrorIfCorrectAnswersAmountIsNotInRange(string type, int correctAmount, int wrongAmount)
        {
            var question = this.generateQuestion(type, correctAmount, wrongAmount);
            var context = new ValidationContext(question);

            try
            {
                validator.Validate(question.Answers, context);
                Assert.Fail("Validator should have thrown a validation error, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }

        [Test]
        [TestCase("single", 1, 0)]
        [TestCase("text", 1, 1)]
        public void Test_CorrectAndWrongAnswersAmountConsidersItAnErrorIfWrongAnswersAmountIsNotInRange(string type, int correctAmount, int wrongAmount)
        {
            var question = this.generateQuestion(type, correctAmount, wrongAmount);
            var context = new ValidationContext(question);

            try
            {
                validator.Validate(question.Answers, context);
                Assert.Fail("Validator should have thrown a validation error, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }

        [Test]
        [TestCase("single", 1, 1)]
        [TestCase("single", 1, 2)]
        [TestCase("multi", 1, 1)]
        [TestCase("multi", 2, 0)]
        [TestCase("text", 1, 0)]
        public void Test_CorrectAndWrongAnswersAmountConsidersItASuccessIfCorrectAndWrongAnswersAmountIsInRange(string type, int correctAmount, int wrongAmount)
        {
            var question = this.generateQuestion(type, correctAmount, wrongAmount);
            var context = new ValidationContext(question);

            try
            {
                validator.Validate(question.Answers, context);
                Assert.Pass();
            }
            catch (ValidationException e)
            {
                Assert.Fail("Validator threw a validation error. Message: " + e.Message);
            }
        }

        [Test]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfTheTypeIsInvalid()
        {
            var question = this.generateQuestion("invalid", 1, 2);
            var context = new ValidationContext(question);

            try
            {
                validator.Validate(question.Answers, context);
                Assert.Fail("Validator should have thrown, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfPassedNull()
        {
            var question = this.generateQuestion("single", 1, 2);
            var context = new ValidationContext(question);

            try
            {
                validator.Validate(null, context);
                Assert.Fail("Validator should have thrown, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfQuestionTypeIsNull()
        {
            var question = this.generateQuestion("single", 1, 2);
            question.Type = null;
            var context = new ValidationContext(question);

            try
            {
                validator.Validate(null, context);
                Assert.Fail("Validator should have thrown, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }
    }
}
