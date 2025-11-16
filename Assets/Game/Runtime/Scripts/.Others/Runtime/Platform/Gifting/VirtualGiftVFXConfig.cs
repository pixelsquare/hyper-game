using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    [CreateAssetMenu(fileName = "GiftingVFX", menuName = "Config/KumuKulitan/Gifting/GiftVFX")]
    public class VirtualGiftVFXConfig : ScriptableObject
    {
        public enum VFXType
        {
            WorldSpace,
            ScreenSpace,
        }

        public VFXType vfxType;
        public GameObject vfxPrefab;
        public float duration;
    }
}
