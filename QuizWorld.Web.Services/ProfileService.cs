using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.UserList;
using QuizWorld.Web.Contracts;
using System.Linq.Expressions;

namespace QuizWorld.Web.Services
{
    public class ProfileService(UserManager<ApplicationUser> userManager) : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<UserViewModel?> GetUserByUsername(string username)
        {

            var user = await _userManager.FindByNameAsync(username);
            if (user is null) return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserViewModel()
            {
                Username = user.UserName!,
                Id = user.Id.ToString(),
                Roles = [.. roles],
            };
        }

        public async Task<ListUsersViewModel> SearchUsers(SearchUsersParameters parameters)
        {
            var predicate = BuildExpression(parameters);
            var query = _userManager.Users
                .Where(predicate);

            int total = await query.CountAsync();

            var users = await query
                .SortByOrder(u => u.UserName, parameters.Order)
                .Paginate(parameters.Page, parameters.PageSize)
                .Select(u => new ListUserItemViewModel()
                {
                    Username = u.UserName!,
                    Id = u.Id.ToString(),
                    Roles = u.UserRoles.Select(ur => ur.Role.Name)!
                })
                .ToListAsync();

            return new ListUsersViewModel()
            {
                Total = total,
                Users = users,
            };
        }

        private static Expression<Func<ApplicationUser, bool>> BuildExpression(SearchUsersParameters parameters)
        {
            var roles = parameters.Roles;
            bool isSearchingForHigherRanks = IsSearchingHigherRanks(parameters);

            if (
                !isSearchingForHigherRanks
                && string.IsNullOrWhiteSpace(parameters.Username))
            {
                return u => true;
            }

            if (isSearchingForHigherRanks
                && string.IsNullOrWhiteSpace(parameters.Username))
            {
                if (roles.Length == 1)
                {
                    return u => u.UserRoles.Any(ur => ur.Role.Name == roles[0]);
                }

                return u => roles.All(role => u.UserRoles.Any(ur => ur.Role.Name == role));
            }

            if (!isSearchingForHigherRanks && !string.IsNullOrWhiteSpace(parameters.Username))
            {
                return u => u.NormalizedUserName == parameters.Username.Normalized();
            }

            return u => u.NormalizedUserName == parameters.Username!.Normalized()
                && roles.All(role => u.UserRoles.Any(ur => ur.Role.Name == role));
        }

        private static bool IsSearchingHigherRanks(SearchUsersParameters parameters)
        {
            var roles = parameters.Roles;

            if (roles.Length == 0)
            {
                return false;
            }

            if (roles.Length == 1 && roles[0] == Roles.User)
            {
                return false;
            }

            return true;
        }
    }
}
