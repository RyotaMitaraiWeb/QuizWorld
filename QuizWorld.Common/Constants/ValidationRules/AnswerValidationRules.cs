using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationRules
{
    /// <summary>
    /// Contains hardcoded values for validation of answer view models. Note that rules about
    /// the amount of answers that a question should have, how many correct/wrong ones a question
    /// must have, and similar are controlled by QuestionValidationRules.
    /// </summary>
    public static class AnswerValidationRules
    {
        public const int MaxLength = 200;
    }
}
