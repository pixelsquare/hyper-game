using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [CreateAssetMenu(menuName = "Config/Hangout/Audio UI SFX Config")]
    public class AudioUISFXConfig : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<SFXClipCode, AudioClip> clipDictionary;

        public SerializableDictionary<SFXClipCode, AudioClip> ClipDictionary => clipDictionary;
        
        public enum SFXClipCode
        {
            CLICK,
            CONFIRM,
            PURCHASE,
        }
    }
}
