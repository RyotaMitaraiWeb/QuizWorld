using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Common.Errors
{
    public static class GradeError
    {
        public enum GradeEntireQuizError
        {
            IsInstantMode = 1,
            QuizOrVersionDoesNotExist,
        }

        public enum GradeQuestionError
        {
            IsNotInstantMode = 1,
            QuestionOrVersionDoesNotExist,
        }
    }
}
