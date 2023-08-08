using QuizWorld.Common.Constants.Types;
using QuizWorld.ViewModels.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Question
{
    /// <summary>
    /// Represents a question in an edit form of a quiz
    /// </summary>
    public class EditQuestionFormViewModel
    {
        public string Prompt = null!;
        public string Type { get; set; } = null!;
        public int Order { get; set; }
        public IEnumerable<EditAnswerFormViewModel> Answers { get; set; } = null!;
    }
}
