using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Result;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Roles;
using QuizWorld.ViewModels.UserList;
using QuizWorld.Web.Contracts;
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
    }
}
