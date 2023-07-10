using QuizWorld.Common.Constants.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationRules
{
    /// <summary>
    /// Contains hardcoded values for validation of question view models. This also includes
    /// things like required/maximum amount of answers and so on.
    /// </summary>
    public static class QuestionValidationRules
    {
        /// <summary>
        /// Enum question types presented as keys derived from the types' short names.
        /// This makes it easier for clients to send the required type, which will be converted to an
        /// enum with model binding.
        /// </summary>
        public static readonly Dictionary<string, QuestionTypes> AllowedTypes =
            new()
            {
                { QuestionTypesShortNames.SingleChoice, QuestionTypes.SingleChoice },
                { QuestionTypesShortNames.MultipleChoice, QuestionTypes.MultipleChoice },
                { QuestionTypesShortNames.Text, QuestionTypes.Text }
            };

        public static class Prompt
        {
            public const int MaxLength = 200;
        }

        public static class SingleChoice
        {
            public const int MinimumAmountOfAnswers = 2;
            public const int MaximumAmountOfAnswers = 10;
        }

        public static class MultipleChoice
        {
            public const int MinimumAmountOfAnswers = 2;
            public const int MaximumAmountOfAnswers = 10;
            public const int MinimumAmountOfCorrectAnswers = 1;
        }

        public static class Text
        {
            public const int MinimumAmountOfAnswers = 1;
            public const int MaximumAmountOfAnswers = 15;
        }
    }
}
