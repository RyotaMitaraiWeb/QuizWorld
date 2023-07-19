using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data.Entities.Logging
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
