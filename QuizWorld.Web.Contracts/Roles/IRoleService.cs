using QuizWorld.Common.Constants.Sorting;
using QuizWorld.ViewModels.UserList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Contracts.Roles
{
    public interface IRoleService
    {
        public Task<Guid?> GiveUserRole(Guid userId, string role);
        public Task<Guid?> GiveUserRole(string userId, string role);

        public Task<Guid?> RemoveRoleFromUser(string userId, string role);
        public Task<Guid?> RemoveRoleFromUser(Guid userId, string role);

        public Task<ListUsersViewModel> GetUsersOfRole(string role, int page, SortingOrders order, int pageSize);
        public Task<ListUsersViewModel> GetUsersByUsername(string query, int page, SortingOrders order, int pageSize = 20);
    }
}
