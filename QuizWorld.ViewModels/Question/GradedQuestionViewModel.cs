using QuizWorld.ViewModels.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Question
{
    /// <summary>
    /// Represents a question with all of its correct answers.
    /// </summary>
    public class GradedQuestionViewModel
    {
        public string Id { get; set; } = string.Empty;
        public IEnumerable<AnswerViewModel> Answers = [];

        [JsonIgnore]
        public bool InstantMode;

        [JsonIgnore]
        public int Version { get; set; }
    }
}
