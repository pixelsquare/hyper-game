using System;

namespace Kumu.Kulitan.Gifting
{
    public interface IVirtualGiftBasicVFXDisplay
    {
        public float Lifetime { get; set; }
        public event Action OnCompleted;

        public void ShowVFX(int multiplier);
    }
}
