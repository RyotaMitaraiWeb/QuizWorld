namespace QuizWorld.Common.Result
{
    public class Result<T, TError> where TError : Enum
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public TError Error { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = default!;
        }

        private Result(TError error)
        {
            IsSuccess = false;
            Error = error;
            Value = default!;
        }

        public static Result<T, TError> Success(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return new Result<T, TError>(value);
        }

        public static Result<T, TError> Failure(TError error)
        {
            if (!typeof(TError).IsEnum)
                throw new ArgumentException("TError must be an enum.");
            return new Result<T, TError>(error);
        }
    }
}
