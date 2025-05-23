namespace QuizWorld.Common.Http
{
    public class HttpError
    {
        public int ErrorCode { get; }

        public HttpError(int errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
