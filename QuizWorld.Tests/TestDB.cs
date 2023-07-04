using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using QuizWorld.Infrastructure.Data;
using QuizWorld.Infrastructure.Data.Entities;
using Microsoft.AspNetCore.Identity;
using NUnit.Framework.Internal;
using Microsoft.Extensions.Logging;
using Moq;

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

            this.userManager.CreateAsync(this.User, "123456").Wait();
            this.userManager.CreateAsync(this.Moderator, "123456").Wait();
            this.userManager.CreateAsync(this.Admin, "123456").Wait();

            this.userManager.AddToRoleAsync(this.User, "User").Wait();
            this.userManager.AddToRolesAsync(this.Moderator, new string[] { "User", "Moderator" }).Wait();
            this.userManager.AddToRolesAsync(this.Admin, new string[] { "User", "Moderator", "Administrator" }).Wait();

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
