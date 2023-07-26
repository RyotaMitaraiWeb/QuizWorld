using QuizWorld.Common.Constants.Sorting;
using QuizWorld.ViewModels.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Contracts.Logging
{
    public interface IActivityLogger
    {
        Task LogActivity(string message, DateTime date);
        Task<ActivityLogsViewModel> RetrieveLogs(int page, SortingOrders order, int pageSize);
    }
}
