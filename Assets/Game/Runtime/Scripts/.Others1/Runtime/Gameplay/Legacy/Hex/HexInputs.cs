using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class HexInputsBase
    {
    }

    public class IdalmunonHexInputs : HexInputsBase
    {
        public float _duration;
        public float _radius;
        public uint _damage;
        public LayerMask _layerMask;
        public Transform _transform;
        public GameObject _indicatorPrefab;
        public GameObject _radiusDisplayPrefab;
    }
}
