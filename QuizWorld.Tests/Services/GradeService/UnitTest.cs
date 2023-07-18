using Moq;
using QuizWorld.Infrastructure;
using QuizWorld.Web.Services.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.GradeServiceUnitTests
{
    public class UnitTest
    {
        public Mock<IRepository> repositoryMock;
        public GradeService service;

        [SetUp]
        public void Setup()
        {
            this.repositoryMock = new Mock<IRepository>();
            this.service = new GradeService(this.repositoryMock.Object);
        }
    }
}
