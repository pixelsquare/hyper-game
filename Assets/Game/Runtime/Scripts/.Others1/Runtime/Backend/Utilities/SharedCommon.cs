namespace Santelmo.Rinsurv.Backend
{
    public abstract class RequestCommon
    {
    }

    public abstract class ResultBase
    {
    }

    public class HttpResponseObject
    {
        public int error_code;
        public string message;
        public object data;
        public object errors;
        public string request_completed_timestamp;
    }

    public class ServiceResultWrapper<T> where T : ResultBase
    {
        public T Result { get; }
        public ServiceError Error { get; }
        public bool HasError => Error != null;

        public ServiceResultWrapper(T result)
        {
            Result = result;
        }

        public ServiceResultWrapper(ServiceError error)
        {
            Error = error;
        }
    }
}
