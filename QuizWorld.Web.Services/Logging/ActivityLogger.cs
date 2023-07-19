﻿using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Logging;
using QuizWorld.ViewModels.Logging;
using QuizWorld.Web.Contracts.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IRepository repository;

        public ActivityLogger(IRepository repository)
        {
            this.repository = repository;
        }

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

            await this.repository.AddAsync(log);
            await this.repository.SaveChangesAsync();
        }

        /// <summary>
        /// Retrieves a paginated and sorted list of activity logs.
        /// </summary>
        /// <param name="page">The current page</param>
        /// <param name="order">The order in which the logs will be sorted. All logs are sorted by their date.</param>
        /// <returns>A paginated and sorted list of activity logs.</returns>
        public Task<IEnumerable<ActivityLogViewModel>> RetrieveLogs(int page, SortingOrders order, int pageSize = 6)
        {
            throw new NotImplementedException();
        }
    }
}
