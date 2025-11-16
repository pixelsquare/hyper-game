using System.Collections.Generic;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class EmoteBubbleController : MonoBehaviour
    {
        [SerializeField] private EmoteBubbleUI emoteBubblePrefab;
        [SerializeField] private Camera targetCamera;

        private Slot<string> eventSlot;
        private Dictionary<EntityRef, EmoteBubbleUI> emoteBubbleMap = new Dictionary<EntityRef, EmoteBubbleUI>();

        private void OnPlayerInitialized(IEvent<string> callback)
        {
            var playerInitializedCallback = (QuantumPlayerJoinedEvent)callback;
            var emoteBubble = Instantiate(emoteBubblePrefab, transform);
            emoteBubble.Initialize(playerInitializedCallback.PlayerTransform, targetCamera);
            emoteBubbleMap.Add(playerInitializedCallback.PlayerEntity, emoteBubble);
        }

        private void OnPlayerDeinitialized(IEvent<string> callback)
        {
            var playerDeinitializedCallback = (QuantumPlayerRemovedEvent)callback;
            if (emoteBubbleMap.TryGetValue(playerDeinitializedCallback.EntityRef, out var bubble))
            {
                Destroy(bubble.gameObject);
                emoteBubbleMap.Remove(playerDeinitializedCallback.EntityRef);
            }
        }

        private void OnPlayEmote(IEvent<string> callback)
        {
            var playerPlayEmoteEvent = (EmoteBubblePlayedEvent)callback;

            if (emoteBubbleMap.TryGetValue(playerPlayEmoteEvent.EntityRef, out var emoteBubble))
            {
                if (playerPlayEmoteEvent.AnimDataAsset is EmotesAnimationAsset emoteAnimAsset)
                {
                    emoteBubble.PlayAnimation(emoteAnimAsset.EmoteBubbleAnimation);
                }
            }
            else
            {
                $"Cannot find entityRef {playerPlayEmoteEvent.EntityRef} in emoteBubbleMap".LogError();
            }
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInitialized);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerDeinitialized);
            eventSlot.SubscribeOn(EmoteBubblePlayedEvent.EVENT_NAME, OnPlayEmote);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}
