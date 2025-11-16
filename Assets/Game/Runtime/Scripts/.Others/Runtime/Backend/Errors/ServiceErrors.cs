namespace Kumu.Kulitan.Backend
{
    public class ServiceError
    {
        public int Code { get; }
        public string Message { get; }
        
        public string RawError { get; }

        // Default constructor needed by VisualScripting
        public ServiceError()
        {
        }

        public ServiceError(int code, string message, string rawError = null)
        {
            Code = code;
            Message = message;
            RawError = rawError;
        }

        public override string ToString()
        {
            return $"[{Code}] | {Message}";
        }
    }

    public static class ServiceErrors
    {
        public static readonly ServiceError unknownError = new(ServiceErrorCodes.UNKNOWN_ERROR, "Unknown error has occured.");

        public static readonly ServiceError networkUnreachableError = new(ServiceErrorCodes.NETWORK_UNREACHABLE_ERROR, "Network is unreachable. Try again later.");

        public static readonly ServiceError networkTimeoutError = new(ServiceErrorCodes.NETWORK_TIMEOUT_ERROR, "Network timed out.");

        public static readonly ServiceError networkUnknownError = new(ServiceErrorCodes.NETWORK_UNKNOWN_ERROR, "Unknown network error has occured.");

        public static readonly ServiceError mismatchedVersionError = new ServiceError(ServiceErrorCodes.APP_VERSION_MISMATCH, "App version mismatch. Please update to the latest version.");
    }
}
