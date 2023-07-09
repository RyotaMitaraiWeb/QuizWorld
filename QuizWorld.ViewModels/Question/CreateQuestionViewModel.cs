using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.ValidationRules;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Validators;

namespace QuizWorld.ViewModels.Question
{
    public class CreateQuestionViewModel
    {
        [Required(ErrorMessage = QuestionValidationErrorMessages.Prompt.IsEmpty)]
        [MaxLength(QuestionValidationRules.Prompt.MaxLength, ErrorMessage = QuestionValidationErrorMessages.Prompt.IsTooLong)]
        public string Prompt { get; set; } = string.Empty;

        public string Type { get; set; } = "single";

        [HasEnoughAnswers]
        public IEnumerable<CreateAnswerViewModel> Answers { get; set; } = Enumerable.Empty<CreateAnswerViewModel>();

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    yield return this.ValidateAmountOfAnswersForSingleChoiceQuestion(this.Answers);
        //}


        //private ValidationResult ValidateAmountOfAnswersForSingleChoiceQuestion(IEnumerable<CreateAnswerViewModel> answers)
        //{
        //    if (answers.Count() < QuestionValidationRules.SingleChoice.MinimumAmountOfAnswers)
        //    {
        //        return new ValidationResult(QuestionValidationErrorMessages.SingleChoice.NotEnoughAnswers);
        //    }

        //    if (answers.Count() > QuestionValidationRules.SingleChoice.MaximumAmountOfAnswers)
        //    {
        //        return new ValidationResult(QuestionValidationErrorMessages.SingleChoice.TooManyAnswers);
        //    }

        //    return ValidationResult.Success;
        //}
    }
}
