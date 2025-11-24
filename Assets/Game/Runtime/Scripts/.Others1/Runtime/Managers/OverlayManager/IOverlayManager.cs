namespace Santelmo.Rinsurv
{
    public interface IOverlayManager : IGlobalBinding
    {
        public void ShowOverlay(string message = null);

        public void HideOverlay();
    }
}
