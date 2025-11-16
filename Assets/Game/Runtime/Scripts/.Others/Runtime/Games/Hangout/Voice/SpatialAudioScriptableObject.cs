using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Spatial Audio Data")]
    public class SpatialAudioScriptableObject : ScriptableObject
    {
        [SerializeField] private float minChatProximity;
        [SerializeField] private float maxChatProximity;

        public float MinChatProximity
        {
            get => minChatProximity;
            set => minChatProximity = value;
        }

        public float MaxChatProximity
        {
            get => maxChatProximity;
            set => maxChatProximity = value;
        }
    }
}
