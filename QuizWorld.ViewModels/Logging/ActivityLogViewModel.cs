using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Logging
{
    /// <summary>
    /// Activity logs show what a moderator or administrator (depending on the policy) did on
    /// a certain date.
    /// </summary>
    public class ActivityLogViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
