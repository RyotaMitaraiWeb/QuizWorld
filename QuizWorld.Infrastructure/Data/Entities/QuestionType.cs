using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data.Entities
{
    [Comment("The type of the question. Questions can be single-choice, multiple-choice, or text-based")]
    public class QuestionType
    {
        [Comment("The ID of the type")]
        [Key]
        public int Id { get; set; }

        [Comment("The type of the question. 0 = single-choice, 1 = multiple-choice, 2 = text")]
        public QuestionTypes Type { get; set; }

        [Comment("The short name of the type, which the client will typically use. The current short names are \"single\", \"multi\", and \"text\"")]
        public string ShortName { get; set; } = null!;

        [Comment("The full name of the type")]
        public string FullName { get; set; } = null!;
    }
}
