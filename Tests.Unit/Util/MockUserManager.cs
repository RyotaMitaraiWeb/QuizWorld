using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using QuizWorld.Infrastructure.Data.Entities.Identity;

namespace QuizWorld.Tests.Unit.Util
{
    public static class MockUserManager
    {
        public static UserManager<ApplicationUser> Get()
        {
            return Substitute.For<UserManager<ApplicationUser>>(
                    Substitute.For<IUserStore<ApplicationUser>>(),
                    Substitute.For<IOptions<IdentityOptions>>(),
                    Substitute.For<IPasswordHasher<ApplicationUser>>(),
                    Array.Empty<IUserValidator<ApplicationUser>>(),
                    Array.Empty<IPasswordValidator<ApplicationUser>>(),
                    Substitute.For<ILookupNormalizer>(),
                    Substitute.For<IdentityErrorDescriber>(),
                    Substitute.For<IServiceProvider>(),
                    Substitute.For<ILogger<UserManager<ApplicationUser>>>()
                );
        }
    }
}
