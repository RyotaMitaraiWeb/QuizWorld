using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationErrorMessages
{
    /// <summary>
    /// Contains static messages that indicate that a quiz view model has failed validations.
    /// </summary>
    public static class QuizValidationErrorMessages
    {
        public static class Title
        {
            public const string IsTooShort = "The quiz title must be at least ten characters!";
            public const string IsTooLong = "The quiz title must be no longer than 200 characters!";
            public const string IsEmpty = "The quiz title is required!";
        }

        public static class Description
        {
            public const string IsTooShort = "The quiz description must be at least ten characters!";
            public const string IsTooLong = "The quiz description must be no longer than 500 characters!";
            public const string IsEmpty = "The quiz description is required!";
        }

        public static class Questions
        {
            public const string NotEnoughQuestions = "The quiz must have at least one question!";
            public const string TooManyQuestions = "The quiz cannot have more than 100 questions!";
        }
    }
}
