namespace CommonContainerRegistry.Exceptions
{
    public class ApiExceptions : Exception
    {
        public readonly string ErrorCode = string.Empty;
        public readonly string ErrorMessage = string.Empty;
        public readonly string ErrorDescription = string.Empty;
        public ApiExceptions() { }
        public ApiExceptions(string errorCode, string errorMessage, string errorDescription)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ErrorDescription = errorDescription;
        }
        public ApiExceptions(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
        public ApiExceptions(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        public ApiExceptions(string errorMessage, Exception innerException) : base(errorMessage, innerException)
        {
            ErrorMessage = errorMessage;
        }
        public ApiExceptions(string errorMessage, string errorCode, string errorDescription, Exception innerException) : base(errorMessage, innerException)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
        }
        public ApiExceptions(string errorMessage, string errorCode, Exception innerException) : base(errorMessage, innerException)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }
    }
}
