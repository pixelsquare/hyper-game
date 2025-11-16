using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftingSFXHandlerBase : MonoBehaviour
    {
        [SerializeField] private AudioUISFXPlayer uiSfxPlayer;
        [SerializeField] private AudioLocalSFXPlayer localSfxPlayer;
        private Slot<string> eventSlot;

        private void OnVirtualGiftPlaySFXEvent(IEvent<string> obj)
        {
            var callbackObj = (OnVirtualGiftPlaySFX) obj;

            //skip if no clip has been assigned
            if (callbackObj.SFXClip == null)
            {
                return;
            }

            if (callbackObj.SFXType == VirtualGiftConfig.SFXClipType.Spatial)
            {
                localSfxPlayer.PlaySFX(callbackObj.AccountId, callbackObj.SFXClip);
            }
            else
            {
                uiSfxPlayer.PlayUISFXClip(callbackObj.SFXClip);
            }
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(OnVirtualGiftPlaySFX.EVENT_NAME, OnVirtualGiftPlaySFXEvent);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
