using System;

namespace Kumu.Kulitan.Backend
{
    [Serializable]
    public class HttpResponseObject
    {
        public int error_code;
        public string message;
        public object data;
        public object errors;
        public string request_completed_timestamp;
    }

    public class RequestCommon { }

    public enum BanType
    {
        Login,
        Agora
    }

    [Serializable]
    public class BannedObject
    {
        public string[] causes;
        public bool is_permanent;
        public string until_timestamp;
    }
}
