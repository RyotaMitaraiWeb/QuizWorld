using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationErrorMessages
{
    /// <summary>
    /// Contains static messages that indicate that an answer view model has failed validations.
    /// </summary>
    public static class AnswerValidationErrorMessages
    {
        public const string IsEmpty = "The answer cannot be empty!";
        public const string IsTooLong = "The answer must be no longer than 100 characters!";
    }
}
