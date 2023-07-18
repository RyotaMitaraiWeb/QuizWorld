using QuizWorld.ViewModels.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Question
{
    /// <summary>
    /// Represents a question with all of its correct answers.
    /// </summary>
    public class GradedQuestionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public IEnumerable<AnswerViewModel> Answers = Enumerable.Empty<AnswerViewModel>();

        [JsonIgnore]
        public bool InstantMode;
    }
}
