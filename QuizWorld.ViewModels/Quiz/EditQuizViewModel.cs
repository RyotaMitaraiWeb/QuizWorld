using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.ValidationRules;
using QuizWorld.ViewModels.Question;
using System.ComponentModel.DataAnnotations;


namespace QuizWorld.ViewModels.Quiz
{
    /// <summary>
    /// Represents a quiz that is being edited.
    /// </summary>
    public class EditQuizViewModel
    {
        [Required(ErrorMessage = QuizValidationErrorMessages.Title.IsEmpty)]
        [MinLength(QuizValidationRules.Title.MinLength, ErrorMessage = QuizValidationErrorMessages.Title.IsTooShort)]
        [MaxLength(QuizValidationRules.Title.MaxLength, ErrorMessage = QuizValidationErrorMessages.Title.IsTooLong)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = QuizValidationErrorMessages.Description.IsEmpty)]
        [MinLength(QuizValidationRules.Title.MinLength, ErrorMessage = QuizValidationErrorMessages.Description.IsTooShort)]
        [MaxLength(QuizValidationRules.Title.MaxLength, ErrorMessage = QuizValidationErrorMessages.Description.IsTooLong)]
        public string Description { get; set; } = string.Empty;

        [MinLength(QuizValidationRules.Questions.MininmumAmount, ErrorMessage = QuizValidationErrorMessages.Questions.NotEnoughQuestions)]
        [MaxLength(QuizValidationRules.Questions.MaximumAmount, ErrorMessage = QuizValidationErrorMessages.Questions.TooManyQuestions)]
        public IEnumerable<CreateQuestionViewModel> Questions { get; set; } = Enumerable.Empty<CreateQuestionViewModel>();
    }
}
