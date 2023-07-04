using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
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
        private readonly IConfiguration config;
        // private readonly RoleManager<IdentityRole<Guid>> roleManager;
        public UserService(
                UserManager<ApplicationUser> userManager,
                IConfiguration config
                //RoleManager<IdentityRole<Guid>> roleManager
            )
        {
            this.userManager = userManager;
            this.config = config;
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

        /// <summary>
        /// Generates a JWT that can be decoded to a UserViewModel
        /// </summary>
        /// <param name="user">The user to be converted to a JWT</param>
        /// <returns>A string representing the JWT</returns>
        public string GenerateJWT(UserViewModel user)
        {
            var secret = this.config["JWT:Secret"];
            var issuer = this.config["JWT:ValidIssuer"];
            var audience = this.config["JWT:ValidAudience"];


            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim("username", user.Username),
                new Claim("roles", JsonConvert.SerializeObject(user.Roles))
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: DateTime.Now.AddDays(1),
                claims: claims,
                signingCredentials: 
                    new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Decodes the provided JWT into a UserViewModel
        /// </summary>
        /// <param name="jwt">The token to be decoded</param>
        /// <returns>A UserViewModel representing the content of the JWT</returns>
        /// <exception cref="InvalidOperationException">If roles is null</exception>
        public UserViewModel DecodeJWT(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var rolesJson = token.Claims.First(t => t.Type == "roles").Value;
            var roles = JsonConvert.DeserializeObject<string[]>(rolesJson);

            if (roles == null)
            {
                throw new InvalidOperationException("roles is null, this is most likely an error from the token itself");
            }

            var user = new UserViewModel()
            {
                Id = token.Claims.First(t => t.Type == "id").Value,
                Username = token.Claims.First(t => t.Type == "username").Value,
                Roles = roles
            };

            return user;
        }
    }
}
