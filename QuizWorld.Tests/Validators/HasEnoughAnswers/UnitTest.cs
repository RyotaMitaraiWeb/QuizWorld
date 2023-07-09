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
        public void Test_HasEnoughAnswersConsidersItAnErrorIfSingleChoiceQuestionHasTooFewAnswers()
        {
            var question = this.generateQuestion("single", 1);
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
        public void Test_HasEnoughAnswersConsidersItAnErrorIfMultipleChoiceQuestionHasTooFewAnswers()
        {
            var question = this.generateQuestion("multi", 1);
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
        public void Test_HasEnoughAnswersConsidersItAnErrorIfTextQuestionHasTooFewAnswers()
        {
            var question = this.generateQuestion("text", 0);
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
        public void Test_HasEnoughAnswersConsidersItAnErrorIfSingleChoiceQuestionHasTooManyAnswers()
        {
            var question = this.generateQuestion("single", 11);
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
        public void Test_HasEnoughAnswersConsidersItAnErrorIfMultipleChoiceQuestionHasTooManyAnswers()
        {
            var question = this.generateQuestion("multiple", 11);
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
        public void Test_HasEnoughAnswersConsidersItAnErrorIfTextQuestionHasTooManyAnswers()
        {
            var question = this.generateQuestion("text", 16);
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
        [TestCase(2)]
        [TestCase(10)]
        public void Test_HasEnoughAnswersConsidersItASuccessIfSingleChoiceQuestionHasEnoughAnswers(int amount)
        {
            var question = this.generateQuestion("single", amount);
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
        [TestCase(2)]
        [TestCase(10)]
        public void Test_HasEnoughAnswersConsidersItASuccessIfMultipleChoiceQuestionHasEnoughAnswers(int amount)
        {
            var question = this.generateQuestion("multi", amount);
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
        [TestCase(1)]
        [TestCase(15)]
        public void Test_HasEnoughAnswersConsidersItASuccessIfTextQuestionHasEnoughAnswers(int amount)
        {
            var question = this.generateQuestion("text", amount);
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
            var question = this.generateQuestion("invalid", 5);
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
            var question = this.generateQuestion("invalid", 5);
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
