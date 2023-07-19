using QuizWorld.Common.Constants.Sorting;
using QuizWorld.ViewModels.Logging;
using QuizWorld.Web.Contracts.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Web.Services.Logging
{
    /// <summary>
    /// A service that tracks the activity of moderators and administrators.
    /// </summary>
    public class ActivityLogger : IActivityLogger
    {
        /// <summary>
        /// Logs an activity that happened on the given <paramref name="date"/> 
        /// </summary>
        /// <param name="message">The content of the log</param>
        /// <param name="date">The date on which the activity happened</param>
        public Task LogActivity(string message, DateTime date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a paginated and sorted list of activity logs.
        /// </summary>
        /// <param name="page">The current page</param>
        /// <param name="order">The order in which the logs will be sorted. All logs are sorted by their date.</param>
        /// <returns>A paginated and sorted list of activity logs.</returns>
        public Task<IEnumerable<ActivityLogViewModel>> RetrieveLogs(int page, SortingOrders order)
        {
            throw new NotImplementedException();
        }
    }
}
