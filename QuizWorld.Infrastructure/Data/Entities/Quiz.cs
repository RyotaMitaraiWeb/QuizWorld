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
    [Comment("A quiz that has many questions, which on their own can have many answers")]
    public class Quiz
    {
        [Comment("The ID of the quiz")]
        [Key]
        public int Id { get; set; }

        [Comment("The title of the quiz")]
        [Required(ErrorMessage = QuizValidationErrorMessages.Title.IsEmpty)]
        [MaxLength(QuizValidationRules.Title.MaxLength, ErrorMessage = QuizValidationErrorMessages.Title.IsTooLong)]
        public string Title { get; set; } = null!;

        [Comment("The title of the quiz with all letters uppercased. Used for searching")]
        public string NormalizedTitle { get; set; } = null!;

        [Required(ErrorMessage = QuizValidationErrorMessages.Description.IsEmpty)]
        [MaxLength(QuizValidationRules.Title.MaxLength, ErrorMessage = QuizValidationErrorMessages.Description.IsTooLong)]
        public string Description { get; set; } = string.Empty;

        [Comment("The version of the quiz. The version starts at 1 and increments every time the quiz is updated. When the quiz is retrieved, only questions that match the quiz's version will be included")]
        public int Version { get; set; } = 1;

        [Comment("In instant mode, the user can check if their answer is correct as soon as they give one. If not in instant mode, the user has to answer all questions before grading them")]
        public bool InstantMode { get; set; }

        [Comment("The date on which the quiz was created.")]
        public DateTime CreatedOn { get; set; }

        [Comment("The date on which the quiz was last updated. Equal to CreatedOn if it has never been updated.")]
        public DateTime UpdatedOn { get; set; }

        [Comment("If marked as deleted, the quiz will not be retrieved at all")]
        public bool IsDeleted { get; set; }

        [Comment("The ID of the user that created the quiz")]
        [ForeignKey(nameof(Creator))]
        public Guid CreatorId { get; set; }
        public ApplicationUser Creator { get; set; } = null!;

        public IEnumerable<Question> Questions { get; set; } = Enumerable.Empty<Question>();
    }
}
