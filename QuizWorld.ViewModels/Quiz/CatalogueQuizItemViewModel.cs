using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Quiz
{
    /// <summary>
    /// Represents a quiz in a catalogue list.
    /// </summary>
    public class CatalogueQuizItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool InstantMode { get; set; }
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// This is equal to CreatedOn if the quiz has never been edited.
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
}
