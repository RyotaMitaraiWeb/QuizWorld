using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Logging;
using QuizWorld.Web.Services.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.ActivityLoggerInMemoryTests
{
    public class InMemory
    {
        public ActivityLogger logger;
        public Repository repository;
        public TestDB testDB;

        [SetUp]
        public void Setup()
        {
            this.testDB = new TestDB();
            this.repository = testDB.repository;
            this.logger = new ActivityLogger(this.repository);
        }

        [Test]
        [TestCase(SortingOrders.Ascending, "Message #1")]
        [TestCase(SortingOrders.Descending, "Message #3")]
        public async Task Test_RetrieveLogsReturnsPaginatedAndSortedLogs(SortingOrders order, string expectedMessage)
        {
            var result = await this.logger.RetrieveLogs(1, order, 1);

            Assert.That(result.Logs.Count(), Is.EqualTo(1));

            var log = result.Logs.First();
            Assert.Multiple(() =>
            {
                Assert.That(log.Message, Is.EqualTo(expectedMessage));
                Assert.That(result.Total, Is.EqualTo(3));
            });
        }

        [Test]
        public async Task Test_LogActivityAddsALogToTheDatabase()
        {
            var date = DateTime.Now;
            await this.logger.LogActivity("test", date);

            var log = await this.repository
                .AllReadonly<ActivityLog>()
                .Where(al => al.Message == "test")
                .FirstAsync();

            Assert.Multiple(() =>
            {
                Assert.That(log.Message, Is.EqualTo("test"));
                Assert.That(log.Date, Is.EqualTo(date));
            });
        }
    }
}
