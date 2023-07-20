using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data
{
    public class QuestionTypesConfigurer : IEntityTypeConfiguration<QuestionType>
    {
        public void Configure(EntityTypeBuilder<QuestionType> builder)
        {
            builder.HasData(
                new QuestionType()
                {
                    Id = 1,
                    Type = QuestionTypes.SingleChoice,
                    FullName = "Single-choice",
                    ShortName = "single",
                },
                new QuestionType()
                {
                    Id = 2,
                    Type = QuestionTypes.MultipleChoice,
                    FullName = "Multiple-choice",
                    ShortName = "multi",
                },
                new QuestionType()
                {
                    Id = 3,
                    Type = QuestionTypes.Text,
                    FullName = "Text",
                    ShortName = "text",
                }
            );
        }
    }
}
