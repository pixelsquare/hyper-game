using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface IHittable
    {
        public bool IsHittable { get; set; }

        public void Hit(uint damage, Vector3 origin);
    }
}
