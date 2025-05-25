using System.ComponentModel.DataAnnotations;

namespace QuizWorld.Common.Search
{
    /// <summary>
    /// Search params when going through a user's quizzes
    /// </summary>
    public class QuizUserParameters : QuizAllParameters
    {
        [Required]
        public string Username { get; set; } = string.Empty;
    }
}
