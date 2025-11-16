using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarAudioHandler : MonoBehaviour
    {
        [SerializeField] protected EntityView myView;
        private Slot<string> eventSlot;
        public abstract void OnClipReady(AudioClip clip, AudioClipConfig clipConfig);

        protected virtual void Start()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(PlayerAvatarLoadedEvent.EVENT_NAME, OnPlayerAvatarLoaded);
        }

        protected virtual void OnPlayerAvatarLoaded(IEvent<string> callback)
        {
            var eventCallback = (PlayerAvatarLoadedEvent)callback;
            var playerView = eventCallback.PlayerEntityView?.gameObject.GetComponent<PlayerView>();

            if (playerView != null && playerView.ModelTransform.GetComponent<AudioListener>() == null)
            {
                var pv = eventCallback.PlayerEntityView.gameObject.GetComponent<PlayerView>();

                if (QuantumRunner.Default.Game.PlayerIsLocal(pv.PlayerRef))
                {
                    pv.ModelTransform.gameObject.AddComponent<AudioListener>();
                }
            }
        }

        protected virtual void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
