namespace Santelmo.Rinsurv.Backend
{
    public class ServiceError
    {
        public int Code { get; }
        public string Message { get; }
        public string RawError { get; }

        public ServiceError(int code, string message, string rawError = null)
        {
            Code = code;
            Message = message;
            RawError = rawError;
        }

        public override string ToString()
        {
            return $"Service Error: [{Code}] - {Message}";
        }
    }

    public static class ServiceErrorHelper
    {
        public readonly static ServiceError UnknownError = new(0, "Unknown error has occured.");
        public readonly static ServiceError NetworkUnreachableError = new(0, "Network is unreachable. Try again later.");
        public readonly static ServiceError NetworkTimeoutError = new(0, "Network timed out.");
        public readonly static ServiceError NetworkUnknownError = new(0, "Unknown network error has occured.");
    }
}
