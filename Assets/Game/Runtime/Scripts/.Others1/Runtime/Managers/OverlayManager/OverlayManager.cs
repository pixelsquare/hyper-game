namespace Santelmo.Rinsurv
{
    public class OverlayManager : IOverlayManager
    {
        private readonly Overlay _overlay;

        public OverlayManager(Overlay overlay)
        {
            _overlay = overlay;
        }

        public void ShowOverlay(string message = null)
        {
            _overlay.gameObject.SetActive(true);
            _overlay.SetMessage(message);
        }

        public void HideOverlay()
        {
            _overlay.gameObject.SetActive(false);
        }
    }
}
