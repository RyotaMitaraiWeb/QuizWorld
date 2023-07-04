using Microsoft.AspNetCore.Identity;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        // private readonly RoleManager<IdentityRole<Guid>> roleManager;
        public UserService(
                UserManager<ApplicationUser> userManager
                //RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            this.userManager = userManager;
            //this.roleManager = roleManager;
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
    }
}
