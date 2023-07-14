using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.ValidationRules;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Validators;

namespace QuizWorld.ViewModels.Quiz
{
    /// <summary>
    /// Represents a quiz that is being created.
    /// </summary>
    public class CreateQuestionViewModel
    {
        [Required(ErrorMessage = QuestionValidationErrorMessages.Prompt.IsEmpty)]
        [MaxLength(QuestionValidationRules.Prompt.MaxLength, ErrorMessage = QuestionValidationErrorMessages.Prompt.IsEmpty)]
        public string Prompt { get; set; } = string.Empty;

        public QuestionTypes Type { get; set; }

        [HasEnoughAnswers]
        [CorrectAndWrongAnswersAmount]
        public IEnumerable<CreateAnswerViewModel> Answers { get; set; } = Enumerable.Empty<CreateAnswerViewModel>();
    }
}
