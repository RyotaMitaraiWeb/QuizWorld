using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using QuizWorld.Common.Constants.ValidationRules;
using QuizWorld.Common.Constants.ValidationErrorMessages;

namespace QuizWorld.ViewModels.Authentication
{
    /// <summary>
    /// Represents the body of a register request. Includes validations.
    /// </summary>
    public class RegisterViewModel
    {
        [Required(ErrorMessage = UserValidationErrorMessages.Username.IsEmpty)]
        [MinLength(UserValidationConstants.Username.MinLength, ErrorMessage = UserValidationErrorMessages.Username.IsTooShort)]
        [MaxLength(UserValidationConstants.Username.MaxLength, ErrorMessage = UserValidationErrorMessages.Username.IsTooLong)]
        [RegularExpression(UserValidationConstants.Username.AlphanumericPattern, ErrorMessage = UserValidationErrorMessages.Username.IsNotAlphanumeric)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = UserValidationErrorMessages.Password.IsEmpty)]
        [MinLength(UserValidationConstants.Passowrd.MinLength, ErrorMessage = UserValidationErrorMessages.Password.IsTooShort)]
        public string Password { get; set; } = string.Empty;
    }
}
