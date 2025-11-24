using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public interface IPopup
    {
        public UniTask<bool> Task { get; }

        public void OnPopupOpen();

        public void OnPopupClose();

        void Cleanup();
    }
}
