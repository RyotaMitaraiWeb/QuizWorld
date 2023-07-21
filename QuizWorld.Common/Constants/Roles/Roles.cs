using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.Roles
{
    /// <summary>
    /// Constants for roles
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// Administrators can check activity logs and can change users' roles to a limited extent. Refer
        /// to <see cref="RolesThatCanBeGivenOrRemoved"/> to see which roles can be added or removed from
        /// users.
        /// </summary>
        public const string Admin = "Administrator";

        /// <summary>
        /// Moderators can delete and edit other users' quizzes.
        /// </summary>
        public const string Moderator = "Moderator";

        /// <summary>
        /// This is the default role of each registered user.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Contains all roles that can be given to or removed from any user. Roles not present
        /// here can only be managed with seeding or at database level. The only exception is the User role,
        /// which is instead given by default to all registered users and cannot be removed at all.
        /// </summary>
        public readonly static string[] RolesThatCanBeGivenOrRemoved = new[]
        {
            Moderator
        };

        public readonly static string[] AvailableRoles = new[]
        {
            Admin, Moderator, User
        };
    }
}
