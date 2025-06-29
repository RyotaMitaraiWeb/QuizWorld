using System.ComponentModel.DataAnnotations;

namespace QuizWorld.ViewModels.Profile
{
    public class DeleteProfilePictureViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
