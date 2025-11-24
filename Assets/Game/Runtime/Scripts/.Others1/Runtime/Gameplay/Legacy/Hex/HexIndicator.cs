using UnityEngine;

namespace Santelmo.Rinsurv
{
    //marker class for indicators; used for pooling
    public class HexIndicator : MonoBehaviour
    {
        [SerializeField] private LegacyLayer _hexType;

        public LegacyLayer Type => _hexType;
    }
}
