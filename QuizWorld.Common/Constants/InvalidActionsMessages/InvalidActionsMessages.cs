using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.InvalidActionsMessages
{
    public static class InvalidActionMessages
    {
        public const string IsNotLoggedIn = "You must be logged in to perform this action!";
        public const string IsNotLoggedOut = "You must be logged out to perform this action!";

        public const string CannotEdit = "You cannot edit this quiz!";
        public const string CannotDelete = "You cannot delete this quiz!";

        public const string CannotBeDemotedFromModerator = "You cannot demote this user because they are not a moderator!";
        public const string CannotBePromotedToModerator = "You cannot promote this user to a moderator because they already are one!";

        /// <summary>
        /// This is used as a catch-all for authorized actions not covered by other properties.
        /// </summary>
        public const string InsufficientPermissionsForRole = "You cannot perform this action because you do not have the correct role!";


        public const string RequestFailed = "Something went wrong with your request! Please try again later!";
    }
}
