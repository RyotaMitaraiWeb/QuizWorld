using QuizWorld.Common.Constants.Types;
using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.ValidationRules;
using QuizWorld.ViewModels.Answer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Validators
{
    /// <summary>
    /// Determines if the given CreateQuestionViewModel has a valid amount of answers. For text questions,
    /// that amount is between 1 and 15; for other types of questions, it's between 2 and 10.
    /// </summary>
    public class HasEnoughAnswers : ValidationAttribute
    {
        private QuestionTypes? questionType { get; set; } = QuestionTypes.SingleChoice;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not IEnumerable<CreateAnswerViewModel> answers)
            {
                return new ValidationResult("Object value is not set to an instance");
            }

            var typeProperty = validationContext.ObjectType.GetProperty("Type");
            var type = typeProperty?.GetValue(validationContext.ObjectInstance, null);

            if (type == null)
            {

                return new ValidationResult("Property \"Type\" is missing.");

            }

            this.questionType = (QuestionTypes) type;

            return this.questionType switch
            {
                QuestionTypes.SingleChoice => this.ValidateAmountOfAnswersForSingleChoiceQuestion(answers),
                QuestionTypes.MultipleChoice => this.ValidateAmountOfAnswersForMultipleChoiceQuestion(answers),
                QuestionTypes.Text => this.ValidateAmountOfAnswersForTextQuestion(answers),
                _ => new ValidationResult("Cannot validate the amount of answers because the question type is invalid"),
            };
        }

        private ValidationResult ValidateAmountOfAnswersForSingleChoiceQuestion(IEnumerable<CreateAnswerViewModel> answers)
        {
            if (answers.Count() < QuestionValidationRules.SingleChoice.MinimumAmountOfAnswers)
            {
                return new ValidationResult(QuestionValidationErrorMessages.SingleChoice.NotEnoughAnswers);
            }

            if (answers.Count() > QuestionValidationRules.SingleChoice.MaximumAmountOfAnswers)
            {
                return new ValidationResult(QuestionValidationErrorMessages.SingleChoice.TooManyAnswers);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateAmountOfAnswersForMultipleChoiceQuestion(IEnumerable<CreateAnswerViewModel> answers)
        {
            if (answers.Count() < QuestionValidationRules.MultipleChoice.MinimumAmountOfAnswers)
            {
                return new ValidationResult(QuestionValidationErrorMessages.MultipleChoice.NotEnoughAnswers);
            }

            if (answers.Count() > QuestionValidationRules.MultipleChoice.MaximumAmountOfAnswers)
            {
                return new ValidationResult(QuestionValidationErrorMessages.MultipleChoice.TooManyAnswers);
            }

            return ValidationResult.Success;
        }

        private ValidationResult ValidateAmountOfAnswersForTextQuestion(IEnumerable<CreateAnswerViewModel> answers)
        {
            if (answers.Count() < QuestionValidationRules.Text.MinimumAmountOfAnswers)
            {
                return new ValidationResult(QuestionValidationErrorMessages.Text.NotEnoughAnswers);
            }

            if (answers.Count() > QuestionValidationRules.Text.MaximumAmountOfAnswers)
            {
                var count = answers.Count();
                return new ValidationResult(QuestionValidationErrorMessages.Text.TooManyAnswers);
            }

            return ValidationResult.Success;
        }
    }
}
