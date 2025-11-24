using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class GemDropper : MonoBehaviour
    {
        [SerializeField] private int _minValue;
        [SerializeField] private int _maxValue;

        public int MinValue => _minValue;
        public int MaxValue => _maxValue;
    }
}
