using QuizWorld.ViewModels.UserList;
using QuizWorld.Web.Contracts.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services.RoleService
{
    public class RoleService : IRoleService
    {
        /// <summary>
        /// Retrieves a paginated list of users that have the specified role.
        /// </summary>
        /// <param name="role">The role for which users will be retrieved</param>
        /// <param name="page">The current page</param>
        /// <param name="pageSize">The amount of users that will be retrieved</param>
        /// <returns>A paginated list of users sorted by their usernames in an alphabetical order.</returns>
        public Task<IEnumerable<ListUserViewModel>?> GetUsersOfRole(string role, int page, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gives the specified role to the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be given to the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        public Task<Guid?> GiveUserRole(Guid userId, string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gives the specified role to the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be given to the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        public Task<Guid?> GiveUserRole(string userId, string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified role from the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be removed from the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        public Task<Guid?> RemoveRoleFromUser(string userId, string role)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified role from the user.
        /// </summary>
        /// <param name="userId">The user</param>
        /// <param name="role">The role to be removed from the user</param>
        /// <returns>The ID of the user if successful, null otherwise</returns>
        public Task<Guid?> RemoveRoleFromUser(Guid userId, string role)
        {
            throw new NotImplementedException();
        }
    }
}
