using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Hangout
{
    [Serializable]
    public struct AudioClipConfig
    {
        public AssetReferenceT<AudioClip> audioReference;
        public AudioPlayType playType;
        [NonSerialized] public string typeCode;

        public enum AudioPlayType
        {
            LOOP,
            TRIGGER
        }
    }
}
