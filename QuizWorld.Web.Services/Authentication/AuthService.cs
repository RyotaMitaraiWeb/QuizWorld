using Microsoft.AspNetCore.Identity;
using QuizWorld.Common.Errors;
using QuizWorld.Common.Result;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts.Authentication;
using static QuizWorld.Common.Errors.AuthError;

namespace QuizWorld.Web.Services.Authentication
{
    public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<Result<UserViewModel, FailedLoginError>> Login(LoginViewModel credentials)
        {
            ApplicationUser? user = await _userManager.FindByNameAsync(credentials.Username);

            if (user is null)
            {
                return Result<UserViewModel, FailedLoginError>
                    .Failure(FailedLoginError.UserDoesNotExist);
            }

            bool passwordsMatch = await PasswordsMatch(credentials.Password, user);
            if (!passwordsMatch)
            {
                return Result<UserViewModel, FailedLoginError>
                    .Failure(FailedLoginError.WrongPassword);
            }

            var roles = await _userManager.GetRolesAsync(user) ?? [];

            var claims = new UserViewModel()
            {
                Id = user.Id.ToString(),
                Username = user.UserName!,
                Roles = [.. roles],
            };

            return Result<UserViewModel, FailedLoginError>
                .Success(claims);
        }

        private async Task<bool> PasswordsMatch(string password, ApplicationUser user)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
