using Cinemachine;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class HangoutCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineFreeLook cameraController;

        private Slot<string> eventSlot;
        
        private void OnPlayerInitialized(IEvent<string> callback)
        {
            var playerInitEvent = (QuantumPlayerJoinedEvent)callback;
            if (!playerInitEvent.IsLocal)
            {
                return;
            }
            cameraController.Follow = playerInitEvent.PlayerTransform;
            cameraController.LookAt = playerInitEvent.PlayerTransform;
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInitialized);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
