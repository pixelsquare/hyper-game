using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class AimIndicator : MonoBehaviour
    {
        private IAimDirection _aimDirection;

        private void Update()
        {
            transform.up = _aimDirection.AimDirection;
        }

        private void Awake()
        {
            _aimDirection = GetComponentInParent<IAimDirection>();
        }
    }
}
