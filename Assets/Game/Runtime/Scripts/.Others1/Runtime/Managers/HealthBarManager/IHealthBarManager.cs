using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface IHealthBarManager : IGlobalBinding
    {
        public void Add(Health health, Vector3 offset = default);
        public void Remove(Health health);
    }
}
