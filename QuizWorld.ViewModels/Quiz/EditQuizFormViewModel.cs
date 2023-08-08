using QuizWorld.ViewModels.Question;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Quiz
{
    /// <summary>
    /// Represents the data that can be edited in a quiz.
    /// This will be sent to the client so that it can populate the edit quiz form.
    /// </summary>
    public class EditQuizFormViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public IEnumerable<EditQuestionFormViewModel> Questions { get; set; } = null!;
    }
}
