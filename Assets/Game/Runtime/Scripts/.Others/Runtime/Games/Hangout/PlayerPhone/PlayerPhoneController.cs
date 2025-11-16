using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Hangout
{
    /// <summary>
    /// Handles methods for the Player Phone
    /// </summary>
    public class PlayerPhoneController : MonoBehaviour
    {
        [SerializeField] private GameObject phoneUI;
        [SerializeField] private Button btnPhone;

        public void Show()
        {
            phoneUI.SetActive(true);
        }

        public void Hide()
        {
            phoneUI.SetActive(false);
        }

        private void Start()
        {
            Hide();
        }

        private void OnEnable()
        {
            btnPhone.onClick.AddListener(Show);
        }

        private void OnDisable()
        {
            btnPhone.onClick.RemoveListener(Show);
        }
    }
}
