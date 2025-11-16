using System.Collections.Generic;

namespace Kumu.Kulitan.Backend
{
    public interface IRemoteDataNotification
    {
        public string From { get; }
        public string To { get; }
        public string MessageId { get; }
        public IDictionary<string, string> Data { get; }
    }
}
