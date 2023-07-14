using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Answer
{
    /// <summary>
    /// Represents an answer in a "game session"
    /// </summary>
    public class AnswerViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
