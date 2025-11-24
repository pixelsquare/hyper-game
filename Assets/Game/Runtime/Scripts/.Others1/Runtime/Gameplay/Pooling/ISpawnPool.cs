using System.Collections.Generic;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface IMonoSpawnPool<T> where T : MonoBehaviour
    {
        public T Prefab { get; }
        public IEnumerable<T> Pool { get; }

        public T Spawn();
        public void Despawn(T spawn);
    }
}
