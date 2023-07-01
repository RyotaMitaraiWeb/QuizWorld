using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationErrorMessages
{
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
            public const string NotEnoughAnswers = "Question is missing answers!";
            public const string TooManyAnswers = "Question should not have more than 15 answers!";
        }

        public static class SingleChoice
        {
            public const string NotEnoughAnswers = "Question is missing answers!";
            public const string TooManyAnswers = "Question should not have more than ten answers!";
        }

        public static class MultipleChoice
        {
            public const string NotEnoughAnswers = "Question is missing answers!";
            public const string TooManyAnswers = "Question should not have more than ten answers!";
        }
    }
}
