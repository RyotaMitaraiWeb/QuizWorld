using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using QuizWorld.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using QuizWorld.Infrastructure;
using QuizWorld.Common.Constants.Types;
using QuizWorld.Infrastructure.Data.Entities.Logging;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Common.Constants.Roles;

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
        public Quiz DeletedQuiz { get; private set; }
        public Quiz NonInstantQuiz { get; private set; }

        public ActivityLog Log1 { get; set; }
        public ActivityLog Log2 { get; set; }
        public ActivityLog Log3 { get; set; }

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
            //var userStore = new UserStore<ApplicationUser, ApplicationRole, QuizWorldDbContext, Guid>(dbContext);
            var userStore = new UserStore<ApplicationUser, ApplicationRole, QuizWorldDbContext, Guid, IdentityUserClaim<Guid>,
            ApplicationUserRole, IdentityUserLogin<Guid>,
            IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>(dbContext);
            var normalizer = new UpperInvariantLookupNormalizer();

            var passwordValidator = new CustomPasswordValidator();

            // logs aren't needed in the tests, so mocking them out of convenience
            var logger = new Mock<ILogger<UserManager<ApplicationUser>>>();
            
            this.userManager = new UserManager<ApplicationUser>(
                userStore, null, hasher, null, 
                new IPasswordValidator<ApplicationUser>[] { passwordValidator }, 
                normalizer, null, null, logger.Object);

            this.repository = new Repository(dbContext);


            dbContext.Roles.Add(new ApplicationRole()
            {
                Name = Roles.User,
                NormalizedName = Roles.User.ToUpper(),
            });

            dbContext.Roles.Add(new ApplicationRole()
            {
                Name = Roles.Moderator,
                NormalizedName = Roles.Moderator.ToUpper(),
            });

            dbContext.Roles.Add(new ApplicationRole()
            {
                Name = Roles.Admin,
                NormalizedName = Roles.Admin.ToUpper(),
            });

            dbContext.SaveChangesAsync().Wait();

            this.User = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                UserName = "ryota1",
                NormalizedUserName = "RYOTA1",
            };

            this.Admin = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
                UserName = "admin1",
                NormalizedUserName = "ADMIN1"
            };

            this.Moderator = new ApplicationUser()
            {
                Id = Guid.NewGuid(),
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
            this.DeletedQuiz = this.CreateSeededQuiz(true);
            this.NonInstantQuiz = this.CreateSeededQuiz(false, false);

            this.userManager.CreateAsync(this.User, "123456").Wait();
            this.userManager.CreateAsync(this.Moderator, "123456").Wait();
            this.userManager.CreateAsync(this.Admin, "123456").Wait();

            this.userManager.AddToRoleAsync(this.User, Roles.User).Wait();
            this.userManager.AddToRolesAsync(this.Moderator, new string[] { Roles.User, Roles.Moderator }).Wait();
            this.userManager.AddToRolesAsync(this.Admin, new string[] { Roles.User, Roles.Moderator, Roles.Admin }).Wait();

            this.repository.AddAsync(this.Quiz).Wait();
            this.repository.AddAsync(this.DeletedQuiz).Wait();
            this.repository.AddAsync(this.NonInstantQuiz).Wait();
            this.repository.SaveChangesAsync().Wait();

            var logs = this.CreateLogs().ToArray();
            var log1 = logs[0];
            var log2 = logs[1];
            var log3 = logs[2];

            this.repository.AddAsync(log1).Wait();
            this.repository.AddAsync(log2).Wait();
            this.repository.AddAsync(log3).Wait();
            this.repository.SaveChangesAsync().Wait();
        }

        private Quiz CreateSeededQuiz(bool deleted = false, bool instantMode = true)
        {
            var date = DateTime.Now;
            var quiz = new Quiz()
            {
                Title = "Cities/Capitals trivia",
                NormalizedTitle = "CITIES/CAPITALSTRIVIA",
                Description = "A small quiz about cities and capital cities",
                Version = 2,
                InstantMode = instantMode,
                CreatorId = this.User.Id,
                IsDeleted = deleted,
                CreatedOn = date,
                UpdatedOn = date.AddDays(1),
                Questions = this.CreateQuestions().ToList()
            };

            return quiz;
            
        }

        private IEnumerable<ActivityLog> CreateLogs()
        {
            var logs = new List<ActivityLog>();
            for (int i = 0; i < 3; i++)
            {
                logs.Add(new ActivityLog()
                {
                    Id = Guid.NewGuid(),
                    Message = $"Message #{i + 1}",
                    Date = DateTime.Now.AddDays(i),
                });
            }

            return logs;
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
