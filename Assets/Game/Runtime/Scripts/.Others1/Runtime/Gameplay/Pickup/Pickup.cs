using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class Pickup : MonoBehaviour
    {
        public delegate void OnPickupSpawn(Pickup pickup);
        public delegate void OnPickupDespawn(Pickup pickup);

        public static event OnPickupSpawn _onSpawn;
        public static event OnPickupDespawn _onDespawn;

        public OnPickupDespawn OnObjectSpawn;

        public void Init()
        {
            _onSpawn.Invoke(this);
        }

        public void DeInit()
        {
            _onDespawn.Invoke(this);
        }
    }
}
