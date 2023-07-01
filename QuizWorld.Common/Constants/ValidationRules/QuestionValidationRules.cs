using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationRules
{
    public static class QuestionValidationRules
    {
        public static class Prompt
        {
            public const int MaxLength = 200;
        }

        public static class SingleChoice
        {
            public const int MaximumAmountOfAnswers = 10;
        }

        public static class MultipleChoice
        {
            public const int MaximumAmountOfAnswers = 10;
        }

        public static class Text
        {
            public const int MaximumAmountOfAnswers = 15;
        }
    }
}
