using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.ViewModels.Logging
{
    public class ActivityLogsViewModel
    {
        public int Total { get; set; }
        public IEnumerable<ActivityLogViewModel> Logs { get; set; } = Enumerable.Empty <ActivityLogViewModel>();
    }
}
