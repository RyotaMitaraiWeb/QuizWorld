using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Answer
{
    /// <summary>
    /// Represents an answer in an edit form of a quiz.
    /// </summary>
    public class EditAnswerFormViewModel
    {
        public string Value = null!;
        public bool Correct;
    }
}
