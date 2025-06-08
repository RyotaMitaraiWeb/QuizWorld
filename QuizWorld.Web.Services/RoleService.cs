using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Result;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Roles;
using QuizWorld.ViewModels.UserList;
using QuizWorld.Web.Contracts.Roles;
using System.Linq.Expressions;
using static QuizWorld.Common.Errors.RoleError;

namespace QuizWorld.Web.Services
{
    public class RoleService(UserManager<ApplicationUser> userManager) : IRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<Result<string, AddRoleError>> GiveUserRole(ChangeRoleViewModel data)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(data.UserId);
            if (user is null)
            {
                return Result<string, AddRoleError>.Failure(AddRoleError.UserDoesNotExist);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(data.Role))
            {
                return Result<string, AddRoleError>.Failure(AddRoleError.UserAlreadyHasRole);
            }

            await _userManager.AddToRoleAsync(user, data.Role);
            return Result<string, AddRoleError>.Success(data.UserId);
        }

        public async Task<Result<string, RemoveRoleError>> RemoveRoleFromUser(ChangeRoleViewModel data)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(data.UserId);
            if (user is null)
            {
                return Result<string, RemoveRoleError>.Failure(RemoveRoleError.UserDoesNotExist);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains(data.Role))
            {
                return Result<string, RemoveRoleError>.Failure(RemoveRoleError.UserDoesNotHaveRoleInFirstPlace);
            }

            await _userManager.RemoveFromRoleAsync(user, data.Role);
            return Result<string, RemoveRoleError>.Success(data.UserId);
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
