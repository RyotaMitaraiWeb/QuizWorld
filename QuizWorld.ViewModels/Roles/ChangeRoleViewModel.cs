using QuizWorld.ViewModels.Validators;
using System.ComponentModel.DataAnnotations;

namespace QuizWorld.ViewModels.Roles
{
    public class ChangeRoleViewModel
    {
        [Required]
        [ValidRole]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

    }
}
