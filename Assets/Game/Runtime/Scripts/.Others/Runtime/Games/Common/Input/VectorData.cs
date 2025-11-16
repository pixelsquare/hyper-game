using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [CreateAssetMenu(menuName = "Config/Common/VectorData")]
    public class VectorData : ScriptableObject
    {
        private Vector2 value;

        public Vector2 Value => value;

        public void SetValue(Vector2 val)
        {
            value = val;
        }

        public void Reset()
        {
            value = Vector2.zero;
        }
    }
}
