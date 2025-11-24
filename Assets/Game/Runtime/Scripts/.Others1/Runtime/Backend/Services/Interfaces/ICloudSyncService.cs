using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv.Backend
{
    public interface ICloudSyncService : IService
    {
        public UniTask<bool> WriteAsync(string key, object value, CancellationToken cancellationToken = default);
        public UniTask<bool> WriteAsync(Dictionary<string, object> data, CancellationToken cancellationToken = default);
        public UniTask<T> ReadAsync<T>(string key, CancellationToken cancellationToken = default);
    }
}
