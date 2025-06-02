using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Common.Search;
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

        [Obsolete("Use SearchLogsParameters overload")]
        Task<ActivityLogsViewModel> RetrieveLogs(int page, SortingOrders order, int pageSize);

        Task<ActivityLogsViewModel> RetrieveLogs(SearchLogsParameters parameters);
    }
}
