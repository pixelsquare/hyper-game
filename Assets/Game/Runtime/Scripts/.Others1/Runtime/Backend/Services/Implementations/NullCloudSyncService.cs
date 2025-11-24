using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv.Backend
{
    public class NullCloudSyncService : ICloudSyncService
    {
        private readonly Dictionary<string, object> _cloudItems = new();

        public async UniTask<bool> WriteAsync(string key, object value, CancellationToken cancellationToken = default)
        {
            await UniTask.CompletedTask;
            _cloudItems[key] = value;
            return true;
        }

        public async UniTask<bool> WriteAsync(Dictionary<string, object> data, CancellationToken cancellationToken = default)
        {
            await UniTask.CompletedTask;

            foreach (var kvp in data)
            {
                _cloudItems[kvp.Key] = kvp.Value;
            }

            return true;
        }

        public async UniTask<T> ReadAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            await UniTask.CompletedTask;

            if (typeof(T) == typeof(Dictionary<,>))
            {
                return _cloudItems is T items ? items : default;
            }

            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}
