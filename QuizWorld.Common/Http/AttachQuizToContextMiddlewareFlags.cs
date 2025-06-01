namespace QuizWorld.Common.Http
{
    public class AttachQuizToContextMiddlewareFlags
    {
        public const string QuizCreatorIdFlag = "quizCreatorId";
        public const string ShouldLogFlag = "shouldLog";
        public const string Metadata = "attachQuizMetadata";

        public string? Method { get; set; } = HttpMethod.Get.Method;
        public int QuizId { get; set; }
        public string? User { get; set; } = string.Empty;
    }
}
