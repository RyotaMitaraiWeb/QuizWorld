using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using QuizWorld.Infrastructure.Data;
using QuizWorld.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using QuizWorld.Infrastructure;
using QuizWorld.Common.Constants.Types;

namespace QuizWorld.Tests
{
    /// <summary>
    /// Configuration class for in-memory database tests. This can be used in every single
    /// in-memory database test.
    /// </summary>
    public class TestDB
    {
        private string uniqueDbName;

        /// <summary>
        /// Allows access the database's Identity tables.
        /// </summary>
        public UserManager<ApplicationUser> userManager;

        /// <summary>
        /// Pass this to your service so that it can access the database.
        /// </summary>
        public Repository repository;

        /// <summary>
        /// Creates the database and seeds it with some data. Each database name is
        /// guaranteed to be random, preventing race conditions.
        /// </summary>
        public TestDB()
        {
            this.uniqueDbName = "quizworld" + DateTime.Now.Ticks.ToString();
            this.Seed();
        }

        public ApplicationUser Admin { get; private set; }
        public ApplicationUser Moderator { get; private set; }
        public ApplicationUser User { get; private set; }

        public Quiz Quiz { get; private set; }

        
        public QuizWorldDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<QuizWorldDbContext>();
            optionsBuilder.UseInMemoryDatabase(this.uniqueDbName);


            return new QuizWorldDbContext(optionsBuilder.Options);
        }

        private void Seed()
        {
            var hasher = new PasswordHasher<ApplicationUser>();
            var dbContext = this.CreateDbContext();
            var userStore = new UserStore<ApplicationUser, IdentityRole<Guid>, QuizWorldDbContext, Guid>(dbContext);
            var normalizer = new UpperInvariantLookupNormalizer();

            var passwordValidator = new CustomPasswordValidator();

            // logs aren't needed in the tests, so mocking them out of convenience
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();
            
            this.userManager = new UserManager<ApplicationUser>(
                userStore, null, hasher, null, 
                new IPasswordValidator<ApplicationUser>[] { passwordValidator }, 
                normalizer, null, null, logger.Object);

            this.repository = new Repository(dbContext);


            dbContext.Roles.Add(new IdentityRole<Guid>()
            {
                Name = "User",
                NormalizedName = "USER",
            });

            dbContext.Roles.Add(new IdentityRole<Guid>()
            {
                Name = "Moderator",
                NormalizedName = "MODERATOR",
            });

            dbContext.Roles.Add(new IdentityRole<Guid>()
            {
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR",
            });

            dbContext.SaveChangesAsync().Wait();

            this.User = new ApplicationUser()
            {
                UserName = "ryota1",
                NormalizedUserName = "RYOTA1",
            };

            this.Admin = new ApplicationUser()
            {
                UserName = "admin1",
                NormalizedUserName = "ADMIN1"
            };

            this.Moderator = new ApplicationUser()
            {
                UserName = "moderator1",
                NormalizedUserName = "MODERATOR1"
            };

            this.repository.AddAsync(new QuestionType()
            {
                Id = 1,
                Type = QuestionTypes.SingleChoice,
                ShortName = QuestionTypesShortNames.SingleChoice,
                FullName = QuestionTypesFullNames.SingleChoice,
            }).Wait();

            this.repository.AddAsync(new QuestionType()
            {
                Id = 2,
                Type = QuestionTypes.MultipleChoice,
                ShortName = QuestionTypesShortNames.MultipleChoice,
                FullName = QuestionTypesFullNames.MultipleChoice,
            }).Wait();

            this.repository.AddAsync(new QuestionType()
            {
                Id = 3,
                Type = QuestionTypes.Text,
                ShortName = QuestionTypesShortNames.Text,
                FullName = QuestionTypesFullNames.Text,
            }).Wait();

            this.repository.SaveChangesAsync().Wait();

            this.Quiz = this.CreateSeededQuiz();


            this.userManager.CreateAsync(this.User, "123456").Wait();
            this.userManager.CreateAsync(this.Moderator, "123456").Wait();
            this.userManager.CreateAsync(this.Admin, "123456").Wait();

            this.userManager.AddToRoleAsync(this.User, "User").Wait();
            this.userManager.AddToRolesAsync(this.Moderator, new string[] { "User", "Moderator" }).Wait();
            this.userManager.AddToRolesAsync(this.Admin, new string[] { "User", "Moderator", "Administrator" }).Wait();

            this.repository.AddAsync(this.Quiz).Wait();
            this.repository.SaveChangesAsync().Wait();
        }

        private Quiz CreateSeededQuiz()
        {
            var date = DateTime.Now;
            var quiz = new Quiz()
            {
                Title = "Cities/Capitals trivia",
                NormalizedTitle = "CITIES/CAPITALS TRIVIA",
                Description = "A small quiz about cities and capital cities",
                Version = 2,
                InstantMode = true,
                CreatorId = this.User.Id,
                CreatedOn = date,
                UpdatedOn = date,
                Questions = this.CreateQuestions().ToList()
            };

            return quiz;
            
        }

        private IEnumerable<Question> CreateQuestions()
        {
            var questions = new List<Question>();
            questions.Add(new Question()
            {
                Prompt = "What is the capital of Mongolia?",
                QuestionTypeId = 1,
                Version = 2,
                Order = 1,
                Answers = new[]
                {
                    new Answer()
                    {
                        Correct = true,
                        Value = "Ulaanbaatar",
                    },
                    new Answer()
                    {
                        Correct = false,
                        Value = "Kathmandu",
                    }
                }
            });

            questions.Add(new Question()
            {
                Prompt = "What are the capitals of South Africa?",
                QuestionTypeId = 2,
                Version = 2,
                Order = 2,
                Answers = new[]
                {
                    new Answer()
                    {
                        Correct = true,
                        Value = "Pretoria"
                    },
                    new Answer()
                    {
                        Correct = true,
                        Value = "Cape Town"
                    },
                    new Answer()
                    {
                        Correct = true,
                        Value = "Bloemfontein"
                    },
                    new Answer()
                    {
                        Correct = false,
                        Value = "Johannesburg"
                    }
                }
            });

            questions.Add(new Question()
            {
                Prompt = "Name one of the only sovereign city-states in modern times (do not use articles like \"the\")",
                QuestionTypeId = 2,
                Version = 2,
                Order = 2,
                Answers = new[]
                {
                    new Answer()
                    {
                        Correct = true,
                        Value = "Vatican City"
                    },
                    new Answer()
                    {
                        Correct = true,
                        Value = "Vatican City State"
                    },
                    new Answer()
                    {
                        Correct = true,
                        Value = "Monaco"
                    },
                    new Answer()
                    {
                        Correct = true,
                        Value = "Principality of Monaco"
                    },
                    new Answer()
                    {
                        Correct = true,
                        Value = "Singapore"
                    },
                    new Answer()
                    {
                        Correct = true,
                        Value = "Republic of Singapore"
                    }
                }
            });

            // this is a question that has existed in the quiz, but has been removed with an update.
            questions.Add(new Question()
            {
                Prompt = "What does \"D.C.\" stand for in the capital city of the United States of America?",
                QuestionTypeId = 1,
                Version = 1,
                Order = 2,
                Answers = new[]
                {
                    new Answer()
                    {
                        Correct = true,
                        Value = "District of Columbia"
                    },
                    new Answer()
                    {
                        Correct = false,
                        Value = "Dirty Cuffs"
                    },
                    new Answer()
                    {
                        Correct = false,
                        Value = "It does not stand for anything"
                    },
                    new Answer()
                    {
                        Correct = false,
                        Value = "Delaware Cubs"
                    },
                    new Answer()
                    {
                        Correct = false,
                        Value = "Democratic Country"
                    },
                    new Answer()
                    {
                        Correct = false,
                        Value = "Don't come"
                    }
                }
            });

            return questions;
        }
    }
    /// <summary>
    /// Passed to the User Manager when instantiating it in the test database.
    /// This is for test purposes only.
    /// </summary>
    internal class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string password)
        {
            if (password.Length < 6)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError()
                {
                    Code = "Password",
                    Description = "Password must have at least six characters"
                }));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
