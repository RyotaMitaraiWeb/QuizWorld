using Microsoft.AspNetCore.Identity;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Result;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
using static QuizWorld.Common.Errors.AuthError;

namespace QuizWorld.Web.Services
{
    public class AuthService(UserManager<ApplicationUser> userManager) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<Result<UserViewModel, FailedLoginError>> LoginAsync(LoginViewModel credentials)
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

        public async Task<Result<UserViewModel, FailedRegisterError>> RegisterAsync(RegisterViewModel body)
        {
            ApplicationUser? existingUser = await _userManager.FindByNameAsync(body.Username);
            if (existingUser is not null)
            {
                return Result<UserViewModel, FailedRegisterError>
                    .Failure(FailedRegisterError.UsernameIsTaken);
            }

            var registerResult = await CreateUser(body);
            if (registerResult.IsFailure)
            {
                return Result<UserViewModel, FailedRegisterError>
                    .Failure(registerResult.Error);
            }

            ApplicationUser newUser = registerResult.Value;

            UserViewModel user = new()
            {
                Id = newUser.Id.ToString(),
                Username = newUser.UserName!,
                Roles = [Roles.User],
            };

            return Result<UserViewModel, FailedRegisterError>
                .Success(user);
        }

        public async Task<bool> CheckIfUsernameIsTakenAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return user is not null;
        }

        private async Task<bool> PasswordsMatch(string password, ApplicationUser user)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        private async Task<Result<ApplicationUser, FailedRegisterError>> CreateUser(RegisterViewModel body)
        {
            ApplicationUser newUser = new()
            {
                UserName = body.Username,
                NormalizedUserName = body.Username.ToUpper(),
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, body.Password);
            if (!result.Succeeded)
            {
                return Result<ApplicationUser, FailedRegisterError>
                    .Failure(FailedRegisterError.Fail);
            }

            IdentityResult addRoleResult = await _userManager.AddToRoleAsync(newUser, Roles.User);
            if (!addRoleResult.Succeeded)
            {
                return Result<ApplicationUser, FailedRegisterError>
                    .Failure(FailedRegisterError.Fail);
            }

            return Result<ApplicationUser, FailedRegisterError>
                .Success(newUser);
        }
    }
}
