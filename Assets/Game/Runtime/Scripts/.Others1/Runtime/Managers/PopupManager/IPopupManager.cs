namespace Santelmo.Rinsurv
{
    public interface IPopupManager : IGlobalBinding
    {
        public T ShowPopupAsync<T>(PopupType popupType) where T : IPopup;

        public void ShowPopup(PopupType popupType);

        public void ClosePopup(PopupType popupType);

        public void CloseLastPopup();

        public void Cleanup();
    }
}
