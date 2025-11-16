using UnityEngine;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;

namespace Kumu.Kulitan.Hangout
{
    public class PlayerUIHandler : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        private static Transform localPlayerCanvasTrans;

        private Slot<string> eventSlot;
        
        public static Transform LocalPlayerCanvasTrans => localPlayerCanvasTrans;

        private void OnPlayerInstantiated(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (QuantumPlayerJoinedEvent)callback;

            var playerIndicator = playerInstantiatedEvent.PlayerTransform.gameObject.GetComponentInChildren<PlayerIndicator>();
            
            var playerName = $"{playerInstantiatedEvent.Nickname}";
            playerIndicator.Initialize(playerName, playerInstantiatedEvent.IsLocal, playerInstantiatedEvent.ModelTransform, mainCamera);
            
            var cameraFacer = playerInstantiatedEvent.PlayerTransform.gameObject.GetComponentInChildren<UIFaceMainCamera>();

            if (playerInstantiatedEvent.IsLocal)
            {
                localPlayerCanvasTrans = cameraFacer.ParentCanvasTransform;
            }
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInstantiated);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
