using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.UI;


namespace Kumu.Kulitan.UI
{
    public class CurrencyShopButtonBlocker : MonoBehaviour
    {
        [SerializeField] private GameObject blocker;
        [SerializeField] private Button shopButton;

        private Slot<string> eventSlot;

        private void CurrencyShopControllerOnOnInitializeFinished(bool isInitialized)
        {
            blocker.SetActive(!isInitialized);
            shopButton.interactable = isInitialized;
        }

        private void OnCurrencyShopBlockerUpdated(IEvent<string> obj)
        {
            var callback = (CurrencyShopBlockerEvent)obj;
            blocker.SetActive(callback.IsBlocked);
            shopButton.interactable = !callback.IsBlocked;
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            CurrencyShopController.OnInitializeFinished += CurrencyShopControllerOnOnInitializeFinished;
            eventSlot.SubscribeOn(CurrencyShopBlockerEvent.EVENT_NAME, OnCurrencyShopBlockerUpdated);
        }
    
        private void OnDisable()
        {
            CurrencyShopController.OnInitializeFinished -= CurrencyShopControllerOnOnInitializeFinished;
        }
    }
}
