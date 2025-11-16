using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.UI
{
    /// <summary>
    /// Sample Error Popup Usage
    /// </summary>
    public class ErrorPopupEvent : MonoBehaviour
    {
        [SerializeField] private string title;
        [SerializeField] private string message;
        [SerializeField] private string btnName;

        [SerializeField] private UnityEvent OnPopupClosed;

        public void ShowErrorPopup()
        {
            IPopup popup = PopupManager.Instance.OpenErrorPopup(title, message, btnName);
            popup.OnClosed += () =>
            {
                OnPopupClosed?.Invoke();
            };
        }
    }
}
