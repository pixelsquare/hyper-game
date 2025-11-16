using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class ObjectPool<T> where T : Component
    {
        private readonly ConcurrentBag<T> _objects;
        private readonly Func<T> _objectGenerator;
        
        public ObjectPool(Func<T> objectGenerator)
        {
            _objectGenerator = objectGenerator ?? throw new ArgumentNullException(nameof(objectGenerator));
            _objects = new ConcurrentBag<T>();
        }

        public T Get() => _objects.TryTake(out T item) ? item : _objectGenerator();
        public void Return(T item) => _objects.Add(item);

        public void Return(T item, float delay)
        {
            Task.Delay(TimeSpan.FromSeconds(delay)).ContinueWith(o =>
            {
                Return(item);
            });
        }
        
        public void Preload(int preloadAmount)
        {
            for (var i = 0; i < preloadAmount; i++)
            {
                Return(_objectGenerator());
            }
        }
    }
}
