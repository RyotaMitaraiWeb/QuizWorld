using Microsoft.AspNetCore.Http;
using QuizWorld.Common.Constants.ValidationRules;
using QuizWorld.ViewModels.Validators;
using System.ComponentModel.DataAnnotations;

namespace QuizWorld.ViewModels.Profile
{
    public class UploadProfilePictureViewModel
    {
        [Required]
        [MaxFileLength(ProfileValidationConstants.ProfilePictureMaxLengthInMegaBytes)]
        [IsImage]
        public IFormFile File { get; set; } = default!;
    }
}
