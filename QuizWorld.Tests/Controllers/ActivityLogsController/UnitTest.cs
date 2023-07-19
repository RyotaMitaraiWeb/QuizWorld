using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.ViewModels.Logging;
using QuizWorld.Web.Areas.Logging.Controllers;
using QuizWorld.Web.Contracts.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Controllers.ActivityLogsControllerUnitTests
{
    public class UnitTest
    {
        public Mock<IActivityLogger> loggerMock;
        public ActivityLogsController controller;

        [SetUp]
        public void Setup()
        {
            this.loggerMock = new Mock<IActivityLogger>();
            this.controller = new ActivityLogsController(this.loggerMock.Object);
        }

        [Test]
        public async Task Test_GetLogsReturnsAListOfLogs()
        {
            var date = DateTime.Now;
            var list = new List<ActivityLogViewModel>()
            {
                new ActivityLogViewModel
                {
                    Id = "a",
                    Message = "test",
                    Date = date
                }
            };

            var mock = list.BuildMock();

            this.loggerMock
                .Setup(l => l.RetrieveLogs(1, SortingOrders.Ascending, 6))
                .ReturnsAsync(mock);

            var result = await this.controller.GetLogs(1, SortingOrders.Ascending) as OkObjectResult;
            var value = result.Value as List<ActivityLogViewModel>;

            Assert.That(value.First().Message, Is.EqualTo("test"));
        }

        [Test]
        public async Task Test_GetLogsReturnsNotFoundIfLoggerThrows()
        {
            this.loggerMock
                .Setup(l => l.RetrieveLogs(1, SortingOrders.Ascending, 6))
                .ThrowsAsync(new Exception());

            var result = await this.controller.GetLogs(1, SortingOrders.Ascending);
            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }
    }
}
