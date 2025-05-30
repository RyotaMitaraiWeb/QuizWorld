using Microsoft.AspNetCore.Http;
using MockQueryable;
using NSubstitute;
using QuizWorld.Common.Http;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Web.Middlewares;
using System.Linq.Expressions;
using Tests.Util;

namespace Tests.Unit.Middlewares
{
    public class AttachQuizToContextMiddlewareTests
    {
        public RequestDelegate next;
        public IRepository Repository;
        public AttachQuizToContextMiddleware AttachQuizToContextMiddleware;

        [SetUp]
        public void Setup()
        {
            next = Substitute.For<RequestDelegate>();
            Repository = Substitute.For<IRepository>();
            AttachQuizToContextMiddleware = new AttachQuizToContextMiddleware(next);
        }

        [Test]
        public async Task Test_WorksCorrectlyIfRequestMatchesAndTheQuizExists()
        {
            var quiz = SampleQuiz.GenerateQuiz(version: 1, instantMode: false, isDeleted: false);
            int quizId = quiz.Id;
            var context = new DefaultHttpContext();
            context.Request.Method = HttpMethod.Put.Method;
            context.Request.Path = "/quiz";
            context.Request.RouteValues["id"] = quizId;

            var creatorId = quiz.CreatorId.ToString();

            var quizzes = new List<Quiz>
            {
                quiz
            }.AsQueryable();

            var query = quizzes.BuildMock();

            Repository.AllReadonly(Arg.Any<Expression<Func<Quiz, bool>>>())
                .Returns(query);

            await AttachQuizToContextMiddleware.InvokeAsync(context, Repository);

            Assert.That(context.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag], Is.EqualTo(creatorId));
            await next.Received(1).Invoke(context);
        }

        [Test]
        public async Task Test_WorksCorrectlyIfRequestMatchesAndTheQuizDoesNotExist()
        {
            var quiz = SampleQuiz.GenerateQuiz(version: 1, instantMode: false, isDeleted: true);
            int quizId = quiz.Id;
            var context = new DefaultHttpContext();
            context.Request.Method = HttpMethod.Put.Method;
            context.Request.Path = "/quiz";
            context.Request.RouteValues["id"] = quizId;

            var creatorId = quiz.CreatorId.ToString();

            var quizzes = new List<Quiz>
            {
                
            }.AsQueryable();

            var query = quizzes.BuildMock();

            Repository.AllReadonly(Arg.Any<Expression<Func<Quiz, bool>>>())
                .Returns(query);

            await AttachQuizToContextMiddleware.InvokeAsync(context, Repository);

            Assert.That(context.Response.StatusCode, Is.EqualTo(404));
            await next.DidNotReceive().Invoke(context);
            Assert.That(context.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag], Is.Null);
        }

        [Test]
        public async Task Test_WorksCorrectlyIfRequestDoesNotMatch()
        {
            var context = new DefaultHttpContext();
            context.Request.Method = HttpMethod.Put.Method;
            context.Request.Path = "/somethingElseEntirely";

            
            await AttachQuizToContextMiddleware.InvokeAsync(context, Repository);

            Assert.That(context.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag], Is.Null);
            await next.Received(1).Invoke(context);
        }

        [TearDown]
        public void Teardown()
        {
            Repository.Dispose();
        }
    }
}
