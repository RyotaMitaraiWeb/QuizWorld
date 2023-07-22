using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.CanChangeRoles
{
    /// <summary>
    /// Checks whether the user has a role that allows them to interact with users' roles.
    /// For security purposes, all unauthorized requests are responded with 404.
    /// </summary>
    public class CanWorkWithRolesRequirement : IAuthorizationRequirement
    {
        public string[] RolesThatCanChangeRoles { get; set; }
        public bool LogActivity { get; set; }
        /// <param name="logActivity">Whether the policy should log the activity if the user is successfully authorized.</param>
        /// <param name="roles">The roles that can change users' roles</param>
        public CanWorkWithRolesRequirement(bool logActivity, params string[] roles)
        {
            this.LogActivity = logActivity;
            this.RolesThatCanChangeRoles = roles;
        }
    }
}
