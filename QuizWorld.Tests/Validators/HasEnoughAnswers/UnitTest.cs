using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Validators;
using System.ComponentModel.DataAnnotations;


namespace QuizWorld.Tests.Validators.HasEnoughAnswersUnitTests
{
    public class UnitTest
    {
        public HasEnoughAnswers validator;
        private IEnumerable<CreateAnswerViewModel> generateAnswers(int amount)
        {
            var answers = new List<CreateAnswerViewModel>();

            for (int i = 0; i < amount; i++)
            {
                answers.Add(new CreateAnswerViewModel()
                {
                    Value = "a",
                    Correct = true,
                });
            }

            return answers;
        }

        private CreateQuestionViewModel generateQuestion(string questionType, int amount)
        {
            var answers = this.generateAnswers(amount);
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
            this.validator = new HasEnoughAnswers();
        }

        [Test]
        [TestCase("single", 1)]
        [TestCase("single", 11)]
        [TestCase("multi", 1)]
        [TestCase("multi", 11)]
        [TestCase("text", 0)]
        [TestCase("text", 16)]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfAnswersAmountIsNotInRange(string type, int amount)
        {
            var question = this.generateQuestion(type, amount);
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
            catch (Exception e)
            {
                Assert.Fail("Validator threw an exception that is not a validation one. Message: " + e.Message);
            }
        }

        [Test]
        [TestCase("single", 2)]
        [TestCase("single", 10)]
        [TestCase("multi", 2)]
        [TestCase("multi", 10)]
        [TestCase("text", 1)]
        [TestCase("text", 15)]
        public void Test_HasEnoughAnswersConsidersItASuccessIfQuestionHasEnoughAnswers(string type, int amount)
        {
            var question = this.generateQuestion(type, amount);
            var context = new ValidationContext(question);

            try
            {
                validator.Validate(question.Answers, context);
                Assert.Pass();
            }
            catch (ValidationException e)
            {
                Assert.Fail("A validation exception was thrown. Mesasge: " + e.Message);
            }
            
        }

        [Test]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfTheTypeIsInvalid()
        {
            var question = this.generateQuestion("invalid", 5);
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
            var question = this.generateQuestion("single", 5);
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
            var question = this.generateQuestion("single", 5);
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
