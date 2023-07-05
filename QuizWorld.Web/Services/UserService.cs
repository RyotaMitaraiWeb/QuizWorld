using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuizWorld.Infrastructure.Data.Contracts;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
using Redis.OM;
using Redis.OM.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtService jwtService;

        public UserService(
                UserManager<ApplicationUser> userManager,
                IJwtService jwtService
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
    }
}
