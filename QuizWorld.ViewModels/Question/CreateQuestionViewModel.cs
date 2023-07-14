using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.ValidationRules;
using QuizWorld.ViewModels.Question;

namespace QuizWorld.ViewModels.Quiz
{
    /// <summary>
    /// Represents a quiz that is being created.
    /// </summary>
    public class CreateQuizViewModel
    {
        [Required(ErrorMessage = QuizValidationErrorMessages.Title.IsEmpty)]
        [MinLength(QuizValidationRules.Title.MinLength, ErrorMessage = QuizValidationErrorMessages.Title.IsTooShort)]
        [MaxLength(QuizValidationRules.Title.MaxLength, ErrorMessage = QuizValidationErrorMessages.Title.IsTooLong)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = QuizValidationErrorMessages.Description.IsEmpty)]
        [MinLength(QuizValidationRules.Title.MinLength, ErrorMessage = QuizValidationErrorMessages.Description.IsTooShort)]
        [MaxLength(QuizValidationRules.Title.MaxLength, ErrorMessage = QuizValidationErrorMessages.Description.IsTooLong)]
        public string Description { get; set; } = string.Empty;

        public bool InstantMode { get; set; } = false;

        [MinLength(QuizValidationRules.Questions.MininmumAmount, ErrorMessage = QuizValidationErrorMessages.Questions.NotEnoughQuestions)]
        [MaxLength(QuizValidationRules.Questions.MaximumAmount, ErrorMessage = QuizValidationErrorMessages.Questions.TooManyQuestions)]
        public IEnumerable<CreateQuestionViewModel> Questions { get; set; } = Enumerable.Empty<CreateQuestionViewModel>();
    }
}
