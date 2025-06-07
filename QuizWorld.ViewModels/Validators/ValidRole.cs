using System.ComponentModel.DataAnnotations;
using static QuizWorld.Common.Constants.Roles.Roles;
namespace QuizWorld.ViewModels.Validators
{
    public class ValidRole : AllowedValuesAttribute
    {
        public ValidRole() : base(RolesThatCanBeGivenOrRemoved)
        {
            
        }
    }
}
