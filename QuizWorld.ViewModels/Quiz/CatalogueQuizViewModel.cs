using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Quiz
{
    /// <summary>
    /// Represents a catalogue of quizzes.
    /// </summary>
    public class CatalogueQuizViewModel
    {
        /// <summary>
        /// The list of quizzes retrieved in the query.
        /// </summary>
        public IEnumerable<CatalogueQuizItemViewModel> Quizzes { get; set; } = Enumerable.Empty<CatalogueQuizItemViewModel>();
        /// <summary>
        /// The total amount of quizzes in the database (if there is some criteria applied to the
        /// query, the total amount of quizzes in the database that meet them).
        /// </summary>
        public int Total;
    }
}
