using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.CanPerformOwnerAction
{
    /// <summary>
    /// Checks if the user is the owner of the quiz or has at least one of the specified roles.
    /// </summary>
    public class CanPerformOwnerActionRequirement : IAuthorizationRequirement
    {
        public readonly string[] RolesThatCanPerformAction;

        /// <param name="roles">The roles that will be authorized for this policy alongside the creator of the quiz</param>
        public CanPerformOwnerActionRequirement(params string[] roles)
        {
            this.RolesThatCanPerformAction = roles;
        }
    }
}
