using QuizWorld.Common.Constants.ValidationRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QuizWorld.Common.Constants.ValidationRules.UserValidationConstants;

namespace QuizWorld.Common.Constants.ValidationErrorMessages
{
    public static class UserValidationErrorMessages
    {
        public static class Username
        {
            public const string IsTooShort = "The username must be at least five characters long!";
            public const string IsTooLong = "The username must be no more than 15 characters long!";
            public const string IsEmpty = "The username is required!";
            public const string IsNotAlphanumeric = "The username must consist only of English letters and/or numbers!";
            public const string AlreadyExists = "The username has already been taken!";
        }

        public static class Password
        {
            public const string IsTooShort = "The password must be at least six characters long!";
        }
    }
}
