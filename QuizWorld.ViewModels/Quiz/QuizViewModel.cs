using QuizWorld.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Quiz
{
    /// <summary>
    /// Represents a quiz that will be interacted with in a "game session".
    /// </summary>
    public class QuizViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool InstantMode { get; set; }
        public IEnumerable<QuestionViewModel> Questions { get; set; } = Enumerable.Empty<QuestionViewModel>();
    }
}
