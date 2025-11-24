using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface IProjectile
    {
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }
        public float Distance { get; set; }
        public uint Damage { get; set; }
        public uint Pierce { get; set; }

        public void OnHit(Transform hitTransform);
    }
}
