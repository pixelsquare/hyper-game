using Kumu.Kulitan.Backend;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Gifting
{
    [CreateAssetMenu(fileName = "VirtualGiftConfig", menuName = "Config/KumuKulitan/Gifting/VirtualGiftConfig")]
    public class VirtualGiftConfig : ScriptableObject
    {
        [SerializeField] private VirtualGiftVFXConfig gifterVFX;
        [SerializeField] private VirtualGiftVFXConfig gifteeVFX;
        [SerializeField] private AudioClip gifterSFXClip;
        [SerializeField] private SFXClipType gifterSFXType;
        [SerializeField] private AudioClip gifteeSFXClip;
        [SerializeField] private SFXClipType gifteeSFXType;
        [SerializeField] private bool isHidden;
        [SerializeField] protected VirtualGiftData data;
        [SerializeField] protected AssetReferenceSprite spriteRef;
        
        
        public VirtualGiftData Data => data;
        public bool IsHidden => isHidden;
        public AssetReferenceSprite SpriteRef => spriteRef;
        public VirtualGiftVFXConfig GifterVFX => gifterVFX;
        public VirtualGiftVFXConfig GifteeVFX => gifteeVFX;
        public AudioClip GifterSFXClip => gifterSFXClip;
        public AudioClip GifteeSFXClip => gifteeSFXClip;
        public SFXClipType GifterSFXType => gifterSFXType;
        public SFXClipType GifteeSFXType => gifteeSFXType;

        public enum SFXClipType
        {
            Spatial,
            UI
        }

        public string GetTypeCode()
        {
            return "";
        }

        public void SetGiftCost(Currency cost)
        {
            data.cost = cost;
        }
    }
}
