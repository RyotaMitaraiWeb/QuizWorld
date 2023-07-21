using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.UserList;
using QuizWorld.Web.Contracts.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services.RoleService
{
    /// <summary>
    /// Manages users' roles and information related to that.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;

        public RoleService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        /// <summary>
        /// Retrieves a paginated list of users that have the specified role.
        /// </summary>
        /// <param name="role">The role for which users will be retrieved</param>
        /// <param name="page">The current page</param>
        /// <param name="pageSize">The amount of users that will be retrieved</param>
        /// <param name="order">The order in which the users will be sorted. Users are sorted by their username.</param>
        /// <returns>A paginated list of users sorted by their usernames.</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<ListUserViewModel>?> GetUsersOfRole(string role, int page, SortingOrders order, int pageSize = 20)
        {
            if (!Roles.AvailableRoles.Contains(role))
            {
                throw new ArgumentException("The provided role does not exist!");
            }

            var users = await this.userManager.Users
                .Where(u => u.UserRoles.Where(ur => ur.Role.Name == role).Any())
                .SortByOrder(u => u.UserName, order)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new ListUserViewModel()
                {
                    Username = u.UserName,
                    Id = u.Id.ToString(),
                    Roles = this.GenerateRoleString(u.UserRoles)
                })
                .ToListAsync();

            return users;
        }

        /// <summary>
        /// Gives the specified role to the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be given to the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Guid?> GiveUserRole(string userId, string role)
        {
            if (!Roles.RolesThatCanBeGivenOrRemoved.Contains(role))
            {
                throw new ArgumentException($"Role \"{role}\" cannot be given to users!");
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("The user does not exist!");
            }

            try
            {
                var result = await this.userManager.AddToRoleAsync(user, role);
                if (result != IdentityResult.Success)
                {
                    return null;
                }

                return user.Id;
            }
            // User already has the role
            catch (NullReferenceException)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Gives the specified role to the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be given to the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Guid?> GiveUserRole(Guid userId, string role)
        {
            return await this.GiveUserRole(userId.ToString(), role);
        }

        /// <summary>
        /// Removes the specified role from the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be removed from the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Guid?> RemoveRoleFromUser(string userId, string role)
        {
            if (!Roles.RolesThatCanBeGivenOrRemoved.Contains(role))
            {
                throw new ArgumentException($"Role \"{role}\" cannot be removed from users!");
            }

            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("The user does not exist!");
            }

            try
            {
                var result = await this.userManager.RemoveFromRoleAsync(user, role);
                if (result != IdentityResult.Success)
                {
                    return null;
                }

                return user.Id;
            }
            // User does not have the role
            catch (NullReferenceException)
            {
                return null;
            }
        }

        /// <summary>
        /// Removes the specified role from the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be removed from the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Guid?> RemoveRoleFromUser(Guid userId, string role)
        {
            return await this.RemoveRoleFromUser(userId.ToString(), role);
        }

        /// <summary>
        /// Generates a string of roles, separated by a comma and space, ordered alphabetically. The role
        /// "User" is removed if there are other roles present.
        /// </summary>
        /// <param name="usersRoles"></param>
        /// <returns></returns>
        private string GenerateRoleString(ICollection<ApplicationUserRole> usersRoles)
        {
            var roles = usersRoles.Select(ur => ur.Role.Name).ToList();
            if (roles.Count > 1)
            {
                roles.Remove(Roles.User);
            }

            return String.Join(", ", roles.OrderBy(r => r));
        }
    }
}
