using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.Types
{
    /// <summary>
    /// Contains human-readable types for a question.
    /// </summary>
    public enum QuestionTypes
    {
        /// <summary>
        /// Single-choice questions provide the user with at least two answers, of which only one is correct.
        /// </summary>
         SingleChoice,

         /// <summary>
         /// Multiple-choice questions provide the user with at least two answers, of which one or more is correct.
         /// </summary>
         MultipleChoice,

         /// <summary>
         /// Text questions provide the user with a text field in which they have to input a correct answer (case insensitive and ignores spaces)
         /// </summary>
         Text
    }
}
