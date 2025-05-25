using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Authentication
{
    /// <summary>
    /// Represents a user session that will be converted into a JWT.
    /// </summary>
    public class UserViewModel
    {
        public string Username { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string[] Roles { get; set; } = [];

    }
}
