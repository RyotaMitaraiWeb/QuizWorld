using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.ValidationErrorMessages;
using QuizWorld.Common.Constants.ValidationRules;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data.Entities
{
    [Comment("A question in the given quiz.")]
    public class Question
    {
        [Comment("The ID of the question")]
        [Key]
        public Guid Id { get; set; }

        [Comment("The content of the question itself (e.g. \"What is the capital of Mongolia?\"")]
        [Required(ErrorMessage = QuestionValidationErrorMessages.Prompt.IsEmpty)]
        [MaxLength(QuestionValidationRules.Prompt.MaxLength, ErrorMessage = QuestionValidationErrorMessages.Prompt.IsTooLong)]
        public string Prompt { get; set; } = string.Empty;

        [ForeignKey(nameof(QuestionType))]
        [Comment("The ID of the type of this question.")]
        public int QuestionTypeId { get; set; }

        [Comment("The type of the question. Questions can be single-choice, multiple-choice, or text-based")]
        public QuestionType QuestionType { get; set; } = null!;

        [Comment("The version of the question. A question will only be retrieved if it matches the quiz's version or specified explicitely")]
        public int Version { get; set; }

        [ForeignKey(nameof(Quiz))]
        [Comment("The ID of the quiz which this question belongs to")]
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        [Comment("The index of the question for the given quiz, used to reliably preserve the order that the creator wants")]
        public int Order { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
