using System.ComponentModel.DataAnnotations;

namespace QuizWorld.Common.Search
{
    public class QuizSearchParameters : QuizAllParameters
    {
        [Required]
        public string Title { get; set; } = string.Empty;
    }
}
