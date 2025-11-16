using UnityEngine;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;

namespace Kumu.Kulitan.Hangout
{
    public class CustomPlayerInitialDataUpdater : MonoBehaviour
    {
        private Transform playerTransform;

        private Slot<string> eventSlot;

        private void UpdateSpawnPoint()
        {
            var config = ConnectionManager.Instance.CurrentLevelConfig;
            config.CustomPlayerInitialData.HasCustomInitTransform = true;
            config.CustomPlayerInitialData.CustomInitPosition = new Vector3(playerTransform.position.x, playerTransform.position.y+5f, playerTransform.position.z);
        }

        private void OnPlayerInstantiated(IEvent<string> callback)
        {
            var playerInstantiatedCallback = (QuantumPlayerJoinedEvent)callback;
            if (playerInstantiatedCallback.IsLocal)
            {
                playerTransform = playerInstantiatedCallback.PlayerTransform;
            }
        }

        private void OnMinigameLoaded(IEvent<string> callback)
        {
            UpdateSpawnPoint();
        }

        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInstantiated);
            eventSlot.SubscribeOn(LoadedMinigameEvent.EVENT_NAME, OnMinigameLoaded);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}
