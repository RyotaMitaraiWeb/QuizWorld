using QuizWorld.ViewModels.Answer;
using System.ComponentModel.DataAnnotations;
using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.Types;

namespace QuizWorld.ViewModels.Validators
{
    /// <summary>
    /// Checks whether the question has a valid amount of correct and wrong answers. This is determined
    /// by the question's type.
    /// 
    /// Single-choice questions must have at least one correct and wrong answer, multiple-choice questions
    /// must have at least one correct answer, and text questions should have exclusively correct answers.
    /// </summary>
    public class CorrectAndWrongAnswersAmount : ValidationAttribute
    {
        private QuestionTypes? questionType { get; set; } = QuestionTypes.SingleChoice;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IEnumerable<CreateAnswerViewModel> answers)
            {
                return new ValidationResult("Object value is not set to an instance");
            }

            var typeProperty = validationContext.ObjectType.GetProperty("Type");


            var questionType = typeProperty?.GetValue(validationContext.ObjectInstance, null);

            if (questionType == null)
            {

                return new ValidationResult("Property \"Type\" is missing.");

            }

            this.questionType = (QuestionTypes)questionType;


            return this.questionType switch
            {
                QuestionTypes.SingleChoice => this.ValidateForSingleChoiceQuestion(answers),
                QuestionTypes.MultipleChoice => this.ValidateForMultipleChoiceQuestion(answers),
                QuestionTypes.Text => this.ValidateForTextQuestion(answers),
                _ => new ValidationResult("Cannot validate the amount of answers because the question type is invalid"),
            };

        }

        private ValidationResult ValidateForSingleChoiceQuestion(IEnumerable<CreateAnswerViewModel> answers)
        {
            var correctAnswers = answers.Where(a => a.Correct);
            if (!correctAnswers.Any())
            {
                return new ValidationResult(QuestionValidationErrorMessages.SingleChoice.DoesNotHaveCorrectAnswers);
            }

            if (correctAnswers.Count() > 1)
            {
                return new ValidationResult(QuestionValidationErrorMessages.SingleChoice.TooManyCorrectAnswers);
            }

            var wrongAnswers = answers.Where(a => !a.Correct);
            if (!wrongAnswers.Any())
            {
                return new ValidationResult(QuestionValidationErrorMessages.SingleChoice.DoesNotHaveWrongAnswers);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateForMultipleChoiceQuestion(IEnumerable<CreateAnswerViewModel> answers)
        {
            var correctAnswers = answers.Where(a => a.Correct);
            if (!correctAnswers.Any())
            {
                return new ValidationResult(QuestionValidationErrorMessages.MultipleChoice.DoesNotHaveCorrectAnswers);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateForTextQuestion(IEnumerable<CreateAnswerViewModel> answers)
        {
            var correctAnswers = answers.Where(a => a.Correct);
            if (!correctAnswers.Any())
            {
                return new ValidationResult(QuestionValidationErrorMessages.Text.DoesNotHaveCorrectAnswers);
            }

            var wrongAnswers = answers.Where(a => !a.Correct);
            if (wrongAnswers.Any())
            {
                return new ValidationResult(QuestionValidationErrorMessages.Text.HasWrongAnswers);
            }

            return ValidationResult.Success;
        }
    }
}
