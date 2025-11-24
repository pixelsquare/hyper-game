using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface IMotor2dPhysics
    {
        public float Mass { get; }
        public CircleCollider2D CircleCollider2D { get; }
    }
}
