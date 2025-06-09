using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Logging;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.ViewModels.Logging;
using QuizWorld.Web.Contracts;

namespace QuizWorld.Web.Services
{
    /// <summary>
    /// A service that tracks the activity of moderators and administrators.
    /// </summary>
    public class ActivityLogger(IRepository repository) : IActivityLogger
    {
        private readonly IRepository _logRepository = repository;

        /// <summary>
        /// Logs an activity that happened on the given <paramref name="date"/> 
        /// </summary>
        /// <param name="message">The content of the log</param>
        /// <param name="date">The date on which the activity happened</param>
        public async Task LogActivity(string message, DateTime date)
        {
            var log = new ActivityLog()
            {
                Message = message,
                Date = date,
            };

            await _logRepository.AddAsync(log);
            await _logRepository.SaveChangesAsync();
        }


        /// <summary>
        /// Retrieves a paginated and sorted list of activity logs.
        /// </summary>
        /// <param name="page">The current page</param>
        /// <param name="order">The order in which the logs will be sorted. All logs are sorted by their date.</param>
        /// <param name="pageSize">The amount of logs to be taken</param>
        /// <returns>A paginated and sorted list of activity logs.</returns>
        [Obsolete("Use SearchLogsParameters overload")]
        public async Task<ActivityLogsViewModel> RetrieveLogs(int page, SortingOrders order, int pageSize = 6)
        {
            int count = 0;
            var query = _logRepository
                .AllReadonly<ActivityLog>();

            count = await query.CountAsync();
            var logs = await query
                .SortByOrder(al => al.Date, order)
                .Paginate(page, pageSize)
                .Select(al => new ActivityLogViewModel
                {
                    Id = al.Id.ToString(),
                    Message = al.Message,
                    Date = al.Date,
                })
                .ToListAsync();

            

            return new ActivityLogsViewModel() { Total = count, Logs = logs };
        }

        public async Task<ActivityLogsViewModel> RetrieveLogs(SearchLogsParameters parameters)
        {
            var query = _logRepository
                .AllReadonly<ActivityLog>();

            int count = await query.CountAsync();

            var logs = await query
                .SortByOrder(al => al.Date, parameters.Order)
                .Paginate(parameters.Page, parameters.PageSize)
                .Select(al => new ActivityLogViewModel
                {
                    Id = al.Id.ToString(),
                    Message = al.Message,
                    Date = al.Date,
                })
                .ToListAsync();

            return new ActivityLogsViewModel { Total = count, Logs = logs };
        }
    }
}
