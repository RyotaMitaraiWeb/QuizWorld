using Microsoft.AspNetCore.Identity;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Contracts.JsonWebToken;


namespace QuizWorld.Web.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtServiceDeprecated jwtService;

        public UserService(
                UserManager<ApplicationUser> userManager,
                IJwtServiceDeprecated jwtService
            )
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
        }

        /// <summary>
        /// Attempts to register a user with the given username and password.
        /// </summary>
        /// <param name="user">A view model of the register input</param>
        /// <returns>
        /// If successful, a view model that can be converted to a JWT.
        /// If unsuccessful, returns null.
        /// </returns>
        public async Task<UserViewModel?> Register(RegisterViewModel user)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = user.Username,
                NormalizedUserName = user.Username.ToUpper()
            };

            var result = await this.userManager.CreateAsync(applicationUser, user.Password);
            if (result == null)
            {
                return null;
            }


            if (!result.Succeeded)
            {
                return null;
            }

            await this.userManager.AddToRoleAsync(applicationUser, "User");

            return new UserViewModel()
            {
                Username = user.Username,
                Id = applicationUser.Id.ToString(),
                Roles = new string[] { "User" }
            };
        }

        /// <summary>
        /// Checks whether the password is correct for the given username.
        /// </summary>
        /// <param name="user">A view model of the login input</param>
        /// <returns>
        /// If successful, returns a view model that can be converted to a JWT.
        /// If unsuccessful, returns null
        /// </returns>
        public async Task<UserViewModel?> Login(LoginViewModel user)
        {
            var applicationUser = await this.userManager.FindByNameAsync(user.Username);
            if (applicationUser == null)
            {
                return null;
            }

            var passwordMatches = await this.userManager.CheckPasswordAsync(applicationUser, user.Password);
            if (!passwordMatches)
            {
                return null;
            }

            var roles = await userManager.GetRolesAsync(applicationUser);

            return new UserViewModel()
            {
                Id = applicationUser.Id.ToString(),
                Username = applicationUser.UserName,
                Roles = roles.ToArray(),
            };
        }

        public async Task<bool> Logout(string jwt)
        {
            var result = await this.jwtService.InvalidateJWT(jwt);
            return result;

        }

        public async Task<bool> CheckIfUsernameIsTaken(string username)
        {
            var user = await this.userManager.FindByNameAsync(username);
            return user != null;
        }

        public async Task<UserViewModel?> GetUser(string id)
        {
            bool isGuid = Guid.TryParse(id, out _);

            if (!isGuid)
            {
                return null;
            }

            var user = await this.userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var roles = await this.userManager.GetRolesAsync(user);
            if (roles.Count > 1)
            {
                roles.Remove(Roles.User);
            }

            var model = new UserViewModel()
            {
                Id = id,
                Username = user.UserName,
                Roles = roles.ToArray(),
            };

            return model;
        }

        public async Task<UserViewModel?> GetUserByUsername(string username)
        {
            ApplicationUser? appUser = await this.userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return null;
            }

            var roles = await this.userManager.GetRolesAsync(appUser);

            var model = new UserViewModel()
            {
                Id = appUser.Id.ToString(),
                Username = appUser.UserName,
                Roles = roles.ToArray(),
            };

            return model;
        }
    }
}
