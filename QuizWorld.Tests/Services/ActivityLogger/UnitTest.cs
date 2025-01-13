using MockQueryable;
using Moq;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Logging;
using QuizWorld.Web.Services.Logging;


namespace QuizWorld.Tests.Services.ActivityLoggerUnitTests
{
    public class UnitTest
    {
        public Mock<IRepository> repositoryMock;
        public ActivityLogger logger;

        [SetUp]
        public void Setup()
        {
            this.repositoryMock = new Mock<IRepository>();
            this.logger = new ActivityLogger(this.repositoryMock.Object);
        }

        [Test]
        [TestCase(SortingOrders.Ascending, "Message #1")]
        [TestCase(SortingOrders.Descending, "Message #3")]
        public async Task Test_RetrieveLogsReturnsPaginatedAndSortedLogs(SortingOrders order, string expectedMessage)
        {
            this.repositoryMock
                .Setup(r => r.AllReadonly<ActivityLog>())
                .Returns(this.GenerateMockLogs(3));

            var result = await this.logger.RetrieveLogs(1, order, 1);
            Assert.Multiple(() =>
            {
                Assert.That(result.Logs.Count, Is.EqualTo(1));
                Assert.That(result.Total, Is.EqualTo(3));
            });
            var log = result.Logs.First();

            Assert.That(log.Message, Is.EqualTo(expectedMessage));
        }

        private IQueryable<ActivityLog> GenerateMockLogs(int times)
        {
            var logs = new List<ActivityLog>();
            for (int i = 0; i < times; i++)
            {
                logs.Add(new ActivityLog()
                {
                    Id = Guid.NewGuid(),
                    Message = $"Message #{i + 1}",
                    Date = DateTime.Now.AddDays(i),
                });
            }

            return logs.BuildMock();
        }
    }
}
