using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class Gem : MonoBehaviour
    {
        [SerializeField] private GemType _gemType;
        
        public uint Value { get; private set; }
        public GemType Type => _gemType;

        public enum GemType
        {
            Blue,
            Yellow,
            Red
        }

        public void Init(uint value)
        {
            Value = value;
        }
    }
}
