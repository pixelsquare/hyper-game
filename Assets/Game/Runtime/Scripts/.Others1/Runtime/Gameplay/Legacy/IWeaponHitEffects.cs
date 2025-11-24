using UnityEngine;

namespace Santelmo.Rinsurv
{
    public delegate void OnWeaponHit(Transform transform);

    public interface IWeaponHit
    {
        public event OnWeaponHit OnWeaponHit;
    }

    public interface IWeaponHitEffect
    {
        public void OnHitEffect(Transform hitTransform);
    }
}
