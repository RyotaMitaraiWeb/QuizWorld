using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.Image;
using QuizWorld.ViewModels.Profile;
using QuizWorld.ViewModels.UserList;
using QuizWorld.Web.Contracts;
using System.Linq.Expressions;

namespace QuizWorld.Web.Services
{
    public class ProfileService(UserManager<ApplicationUser> userManager, IImageService imageService) : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IImageService _imageService = imageService;

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
                return u => u.NormalizedUserName!.Contains(parameters.Username.Normalized());
            }

            return u => u.NormalizedUserName!.Contains(parameters.Username!.Normalized())
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

        public async Task<UploadedProfilePictureViewModel> UploadProfilePicture(UploadProfilePictureViewModel data, string user)
        {
            UploadImageViewModel metadata = new()
            {
                Directory = Destination,
                Image = data.File,
                FileName = GenerateFileName(user),
                ResizeOptions = new()
                {
                    Width = AvatarSize,
                    Height = AvatarSize,
                }
            };

            var result = await _imageService.UploadImage(metadata);
            return new UploadedProfilePictureViewModel()
            {
                Path = result.Name,
            };
        }

        private readonly string Destination = "profile-pictures";
        /// <summary>
        /// Described in pixels
        /// </summary>
        private readonly int AvatarSize = 256;
        private string GenerateFileName(string user) => $"{user}.{_imageService.DefaultFileExtension}";

        public async Task<DeletedProfilePictureViewModel> DeleteProfilePicture(DeleteProfilePictureViewModel data)
        {
            var metadata = new DeleteImageViewModel()
            {
                Directory = Destination,
                FileName = GenerateFileName(data.Username),
            };

            var result = await _imageService.DeleteImage(metadata);
            return new DeletedProfilePictureViewModel()
            {
                ProfilePictureExisted = result.ImageExisted,
            };
        }
    }
}
