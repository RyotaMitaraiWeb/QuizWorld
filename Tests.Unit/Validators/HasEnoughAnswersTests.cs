using QuizWorld.Common.Constants.Types;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.ViewModels.Validators;
using System.ComponentModel.DataAnnotations;


namespace Tests.Unit.Validators
{
    public class HasEnoughAnswersTests
    {
        public HasEnoughAnswers Validator;
        private static List<CreateAnswerViewModel> GenerateAnswers(int amount)
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

        private static CreateQuestionViewModel GenerateQuestion(QuestionTypes type, int amount)
        {
            var answers = GenerateAnswers(amount);
            return new CreateQuestionViewModel()
            {
                Prompt = "valid prompt",
                Answers = answers,
                Type = type
            };
        }

        [SetUp]
        public void Setup()
        {
            Validator = new HasEnoughAnswers();
        }

        [Test]
        [TestCase(QuestionTypes.SingleChoice, 1)]
        [TestCase(QuestionTypes.SingleChoice, 11)]
        [TestCase(QuestionTypes.MultipleChoice, 1)]
        [TestCase(QuestionTypes.MultipleChoice, 11)]
        [TestCase(QuestionTypes.Text, 0)]
        [TestCase(QuestionTypes.Text, 16)]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfAnswersAmountIsNotInRange(QuestionTypes type, int amount)
        {
            var question = GenerateQuestion(type, amount);
            var context = new ValidationContext(question);

            try
            {
                Validator.Validate(question.Answers, context);
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
        [TestCase(QuestionTypes.SingleChoice, 2)]
        [TestCase(QuestionTypes.SingleChoice, 10)]
        [TestCase(QuestionTypes.MultipleChoice, 2)]
        [TestCase(QuestionTypes.MultipleChoice, 10)]
        [TestCase(QuestionTypes.Text, 1)]
        [TestCase(QuestionTypes.Text, 15)]
        public void Test_HasEnoughAnswersConsidersItASuccessIfQuestionHasEnoughAnswers(QuestionTypes type, int amount)
        {
            var question = GenerateQuestion(type, amount);
            var context = new ValidationContext(question);

            try
            {
                Validator.Validate(question.Answers, context);
                Assert.Pass();
            }
            catch (ValidationException e)
            {
                Assert.Fail("A validation exception was thrown. Mesasge: " + e.Message);
            }
            
        }

        [Test]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfPassedNull()
        {
            var question = GenerateQuestion(QuestionTypes.SingleChoice, 5);
            var context = new ValidationContext(question);

            try
            {
                Validator.Validate(null, context);
                Assert.Fail("Validator should have thrown, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }
    }
}
