using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.UI
{
    public class SocialScreenHelper : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        private Slot<string> eventSlot;
        private string sceneName;

        private void OnActivatePanel(IEvent<string> callback)
        {
            canvasGroup.interactable = sceneName == SceneManager.GetActiveScene().name;
            canvasGroup.blocksRaycasts = sceneName == SceneManager.GetActiveScene().name;
        }
        
        private void OnEnable()
        {
            eventSlot.SubscribeOn(OnActivateMenuPanel.EVENT_NAME, OnActivatePanel);
        }

        private void OnDisable()
        {
            eventSlot.UnSubscribeFor(OnActivateMenuPanel.EVENT_NAME, OnActivatePanel);
        }

        private void OnDestroy()
        {
            GlobalNotifier.Instance.Trigger(new MenuPanelChangedEvent(SceneNames.USER_PROFILE_SCREEN));
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            sceneName = gameObject.scene.name;
        }
    }
}
