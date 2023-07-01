using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationErrorMessages
{
    /// <summary>
    /// Contains static messages that indicate that a question view model has failed validations.
    /// </summary>
    public static class QuestionValidationErrorMessages
    {
        public const string InvalidType = "The question type is invalid! Make sure to provide it as \"single\", \"multi\", or \"text\"";
        public static class Prompt
        {
            public const string IsEmpty = "The question prompt is required!";
            public const string IsTooLong = "The question prompt must be no longer than 200 characters!";
        }

        public static class Text
        {
            public const string NotEnoughAnswers = "Text questions must have at least one answer!";
            public const string TooManyAnswers = "Text questions should not have more than 15 answers!";
            public const string HasWrongAnswers = "Text questions cannot have wrong answers!";
        }

        public static class SingleChoice
        {
            public const string NotEnoughAnswers = "Single-choice questions must have at least two answers!";
            public const string DoesNotHaveWrongAnswers = "Single-choice questions must have at least one wrong answer!";
            public const string TooManyCorrectAnswers = "Single-choice questions should have only one correct answer!";
            public const string TooManyAnswers = "Single-choice questions should not have more than ten answers!";
        }

        public static class MultipleChoice
        {
            public const string NotEnoughAnswers = "Multiple-choice questions must have at least two answers!";
            public const string DoesNotHaveCorrectAnswers = "Multiple-choice questions questions must have at least one correct answer!";
            public const string TooManyAnswers = "Multiple-choice questions should not have more than ten answers!";
        }
    }
}
