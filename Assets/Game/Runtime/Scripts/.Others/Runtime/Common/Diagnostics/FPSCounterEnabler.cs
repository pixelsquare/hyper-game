using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;

public class FPSCounterEnabler : MonoBehaviour
{
    [SerializeField] private GameObject counter;
    private Slot<string> eventSlot;
    
    private void OnShowHideCounter(IEvent<string> callback)
    {
        var eventCallback = (FPSCounterShowHideEvent)callback;
        counter.SetActive(eventCallback.ToShow);
    }

    private void OnEnable()
    {
        eventSlot = new Slot<string>(GlobalNotifier.Instance);
        eventSlot.SubscribeOn(FPSCounterShowHideEvent.EVENT_NAME, OnShowHideCounter);
    }

    private void OnDisable()
    {
        eventSlot.Dispose();
    }
}
