using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants
{
    public static class SuccessfulActionsMessages
    {
        public const string Registered = "You registered successfully!";
        public const string LoggedIn = "You logged in successfully!";
        public const string LoggedOut = "You logged out successfully!";

        public const string CreatedQuiz = "Your quiz was created successfully!";
        public const string EditedQuiz = "Your quiz was edited successfully!";
        public const string DeletedQuiz = "Your quiz was deleted successfully!";

        public const string PromotedToModerator = "You successfully promoted this user to a moderator!";
        public const string DemotedToUser = "You successfuly demoted this moderator to a user!";
    }
}
