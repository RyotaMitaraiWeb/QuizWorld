using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.Infrastructure.Data.Entities.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data
{
    public class QuizWorldDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public QuizWorldDbContext(DbContextOptions<QuizWorldDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfigurer());
            builder.ApplyConfiguration(new QuestionTypesConfigurer());
        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}
