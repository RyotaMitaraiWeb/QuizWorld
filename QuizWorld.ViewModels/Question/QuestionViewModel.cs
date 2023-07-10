using QuizWorld.ViewModels.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Question
{
    /// <summary>
    /// Represents a question in a "game session"
    /// </summary>
    public class QuestionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public IEnumerable<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

    }
}
