using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace QuizWorld.Common.Constants.ValidationRules
{
    public static class UserValidationConstants
    {
        public static class Username
        {
            public const int MinLength = 5;
            public const int MaxLength = 15;
            public const string AlphanumericPattern = @"^[a-zA-Z0-9]+$";
        }
        public static class Passowrd
        {
            public const int MinLength = 6;
        }
    }
}
