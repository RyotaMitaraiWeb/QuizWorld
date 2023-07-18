using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.CanAccessLogs
{
    /// <summary>
    /// Checks whether the user has a role that can access activity logs. 
    /// For security purposes, all unauthorized requests are responded with 404.
    /// </summary>
    public class CanAccessLogsRequirement : IAuthorizationRequirement
    {
        public string[] RolesThatCanAccessLogs { get; set; }
        /// <param name="roles">The roles that can access the activity logs</param>
        public CanAccessLogsRequirement(params string[] roles)
        {
            this.RolesThatCanAccessLogs = roles;
        }
    }
}
