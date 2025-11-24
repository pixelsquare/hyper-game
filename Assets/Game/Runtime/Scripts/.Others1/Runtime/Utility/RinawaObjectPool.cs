using System.Collections.Generic;
using UniRx.Toolkit;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class RinawaObjectPool<T> : ObjectPool<T> where T : Component
    {
        protected T _prefab;
        protected Transform _parent;
        protected DiContainer _diContainer;

        protected List<T> _cachedObjects = new();

        protected RinawaObjectPool(DiContainer diContainer, T prefab, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _diContainer = diContainer;
        }

        public void ReturnAll()
        {
            _cachedObjects.ForEach(Return);
            _cachedObjects.Clear();
            Clear();
        }

        protected override T CreateInstance()
        {
            var obj = _diContainer.InstantiatePrefabForComponent<T>(_prefab, _parent);
            _cachedObjects.Add(obj);
            return obj;
        }
    }
}
