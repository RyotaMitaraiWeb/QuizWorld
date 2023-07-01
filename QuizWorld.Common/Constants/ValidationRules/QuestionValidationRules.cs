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
        public static readonly string[] AllowedTypes = new string[]
        {
            QuestionTypes.MultipleChoice,
            QuestionTypes.SingleChoice,
            QuestionTypes.Text,
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
        }

        public static class Text
        {
            public const int MinimumAmountOfAnswers = 1;
            public const int MaximumAmountOfAnswers = 15;
        }
    }
}
