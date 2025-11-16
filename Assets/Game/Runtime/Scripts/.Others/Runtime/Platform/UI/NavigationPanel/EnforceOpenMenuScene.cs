using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.UI
{
    public class EnforceOpenMenuScene : MonoBehaviour
    {
        private int currentOpenSceneIdx;
        private string currentActiveScene;
        
        private Slot<string> eventSlot;

        public void SetSceneActive()
        {
            if (string.IsNullOrEmpty(currentActiveScene))
            {
                return;
            }
            
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentActiveScene));
            GlobalNotifier.Instance.Trigger(new OnActivateMenuPanel());
        }

        public void SetOpenScene(string newOpenScene)
        {
            currentActiveScene = newOpenScene;
        }
        
        private void OnOpenPanelChanged(IEvent<string> callback)
        {
            var eventCallback = (MenuPanelChangedEvent)callback;
            currentActiveScene = eventCallback.OpenPanelName;
        }
        
        private void OnEnable()
        {
            eventSlot.SubscribeOn(MenuPanelChangedEvent.EVENT_NAME, OnOpenPanelChanged);
        }

        private void OnDisable()
        {
            eventSlot.UnSubscribeFor(MenuPanelChangedEvent.EVENT_NAME, OnOpenPanelChanged);
        }
        
        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }
    }
}
