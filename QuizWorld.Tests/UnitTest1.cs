using Moq;

namespace QuizWorld.Tests
{
    public interface IService
    {
        string doSomething();
    }

    public class Service : IService
    {
        public string doSomething()
        {
            return "a";
        }
    }
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var a = 2;
            Assert.Pass();
        }

        [Test]
        public void MockTest()
        {
            var mock = new Mock<IService>();
            
            mock.Setup(m => m.doSomething())
                .Returns("b");

            IService service = mock.Object;
            var test = service.doSomething();
            Assert.That(test, Is.EqualTo("b"));

            mock.VerifyAll();
        }
    }
}