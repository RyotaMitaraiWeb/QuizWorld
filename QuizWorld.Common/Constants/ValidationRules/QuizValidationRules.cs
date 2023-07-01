using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Constants.ValidationRules
{
    /// <summary>
    /// Contains hardcoded values for validation of quiz view models.
    /// </summary>
    public static class QuizValidationRules
    {
        public static class Title
        {
            public const int MinLength = 10;
            public const int MaxLength = 200;
        }

        public static class Description
        {
            public const int MinLength = 10;
            public const int MaxLength = 500;
        }

        public static class Questions
        {
            public const int MininmumAmount = 1;
            public const int MaximumAmount = 100;
        }

    }
}
