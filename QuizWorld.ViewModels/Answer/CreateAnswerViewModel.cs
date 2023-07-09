using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.ValidationRules;

namespace QuizWorld.ViewModels.Answer
{
    public class CreateAnswerViewModel
    {
        [Required(ErrorMessage = AnswerValidationErrorMessages.IsEmpty)]
        [MaxLength(AnswerValidationRules.MaxLength, ErrorMessage = AnswerValidationErrorMessages.IsTooLong)]
        public string Value { get; set; } = string.Empty;

        public bool Correct { get; set; }
    }
}
