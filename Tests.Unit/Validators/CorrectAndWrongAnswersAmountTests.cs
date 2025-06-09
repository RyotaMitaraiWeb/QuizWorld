using QuizWorld.Common.Constants.Types;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.ViewModels.Validators;
using System.ComponentModel.DataAnnotations;

namespace Tests.Unit.Validators
{
    public class CorrectAndWrongAnswersAmountTests
    {
        public CorrectAndWrongAnswersAmount Validator;
        private static List<CreateAnswerViewModel> GenerateAnswers(int correctAmount, int wrongAmount)
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

        private static CreateQuestionViewModel GenerateQuestion(QuestionTypes questionType, int correctAmount, int wrongAmount)
        {
            var answers = GenerateAnswers(correctAmount, wrongAmount);
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
            Validator = new CorrectAndWrongAnswersAmount();
        }

        [Test]
        [TestCase(QuestionTypes.SingleChoice, 2, 1)]
        [TestCase(QuestionTypes.SingleChoice, 0, 1)]
        [TestCase(QuestionTypes.MultipleChoice, 0, 1)]
        [TestCase(QuestionTypes.Text, 0, 0)]
        public void Test_CorrectAndWrongAnswersAmountConsidersItAnErrorIfCorrectAnswersAmountIsNotInRange(QuestionTypes type, int correctAmount, int wrongAmount)
        {
            var question = GenerateQuestion(type, correctAmount, wrongAmount);
            var context = new ValidationContext(question);

            try
            {
                Validator.Validate(question.Answers, context);
                Assert.Fail("Validator should have thrown a validation error, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(QuestionTypes.SingleChoice, 1, 0)]
        [TestCase(QuestionTypes.Text, 1, 1)]
        public void Test_CorrectAndWrongAnswersAmountConsidersItAnErrorIfWrongAnswersAmountIsNotInRange(QuestionTypes type, int correctAmount, int wrongAmount)
        {
            var question = GenerateQuestion(type, correctAmount, wrongAmount);
            var context = new ValidationContext(question);

            try
            {
                Validator.Validate(question.Answers, context);
                Assert.Fail("Validator should have thrown a validation error, but passed");
            }
            catch (ValidationException)
            {
                Assert.Pass();
            }
        }

        [Test]
        [TestCase(QuestionTypes.SingleChoice, 1, 1)]
        [TestCase(QuestionTypes.SingleChoice, 1, 2)]
        [TestCase(QuestionTypes.MultipleChoice, 1, 1)]
        [TestCase(QuestionTypes.MultipleChoice, 2, 0)]
        [TestCase(QuestionTypes.Text, 1, 0)]
        public void Test_CorrectAndWrongAnswersAmountConsidersItASuccessIfCorrectAndWrongAnswersAmountIsInRange(QuestionTypes type, int correctAmount, int wrongAmount)
        {
            var question = GenerateQuestion(type, correctAmount, wrongAmount);
            var context = new ValidationContext(question);

            try
            {
                Validator.Validate(question.Answers, context);
                Assert.Pass();
            }
            catch (ValidationException e)
            {
                Assert.Fail("Validator threw a validation error. Message: " + e.Message);
            }
        }

        [Test]
        public void Test_HasEnoughAnswersConsidersItAnErrorIfPassedNull()
        {
            var question = GenerateQuestion(QuestionTypes.SingleChoice, 1, 2);
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
