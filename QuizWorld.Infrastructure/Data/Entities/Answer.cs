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
    [Comment("An answer for the given question, which can be correct or wrong. For text questions, all answers should be correct")]
    public class Answer
    {
        [Comment("Primary key for the answer. For single- and multiple-choice questions, the ID is also used on the client to determine if the user has answered the question correctly")]
        [Key]
        public Guid Id { get; set; }

        [Comment("The content of the answer. For text questions, this is also used on the client to determine whether the user has answered correctly")]
        [Required(ErrorMessage = AnswerValidationErrorMessages.IsEmpty)]
        [MaxLength(AnswerValidationRules.MaxLength, ErrorMessage = AnswerValidationErrorMessages.IsTooLong)]
        public string Value { get; set; } = string.Empty;

        [Comment("A mark that indicates whether the answer is considered correct or wrong. Text questions should have only correct answers.")]
        public bool Correct { get; set; }

        [Comment("The ID of the question which this answer belongs to")]
        [ForeignKey(nameof(Question))]
        public Guid QuestionId { get; set; }
        public Question Question { get; set; } = null!;
    }
}
