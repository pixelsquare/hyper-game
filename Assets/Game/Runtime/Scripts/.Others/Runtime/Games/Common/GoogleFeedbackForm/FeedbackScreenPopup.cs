using UnityEngine;
using UnityEngine.UI;
using Kumu.Kulitan.Common;

namespace Kumu.Kulitan.UI
{
    public class FeedbackScreenPopup : BasePopup
    {
        [SerializeField] private Button closeButton;

        public void OnPopupClose()
        {
            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            PopupManager.Instance.CloseScenePopup(SceneNames.FEEDBACK_SCREEN);
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener(OnPopupClose);
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnPopupClose);
        }
    }
}
