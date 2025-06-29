using QuizWorld.Web.Contracts;

namespace QuizWorld.Web.Services
{
    public class ContentRootPathProvider(string contentRootPath) : IContentRootPathProvider
    {
        public string Path { get; } = contentRootPath;
    }
}
