using System.Collections.Generic;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class AudioDuckingPoller : MonoBehaviour
    {
        [SerializeField] private float duckAttenuationValue = -30f;
        [SerializeField] private int volumeCutoff;
        private const string DUCK_ATTENUATION_PARAM = "volume";
        private Slot<string> eventSlot;
        private List<HeadAnimation> talkingHeads;
        private int activeTalkingHeadCount;

        void Update()
        {
            if (talkingHeads.Count > 0)
            {
                activeTalkingHeadCount = 0;

                if (!PlayerPrefs.HasKey(AudioSettingsApplicator.VOICE_VOLUME_KEY) || 
                    PlayerPrefs.GetFloat(AudioSettingsApplicator.VOICE_VOLUME_KEY) > 0)
                {
                    foreach (var head in talkingHeads)
                    {
                        if (head.CurrentVolume > volumeCutoff)
                        {
                            activeTalkingHeadCount += 1;
                        }
                    }
                }

                if (activeTalkingHeadCount > 0)
                {
                    AudioSettingsApplicator.Instance.Mixer.SetFloat(DUCK_ATTENUATION_PARAM, duckAttenuationValue);
                }
                else
                {
                    AudioSettingsApplicator.Instance.Mixer.SetFloat(DUCK_ATTENUATION_PARAM, 0);
                }
            }
        }

        private void Awake()
        {
            talkingHeads = new List<HeadAnimation>();

            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoin);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerLeave);
        }

        private void OnPlayerJoin(IEvent<string> callback)
        {
            var joinEvent = (QuantumPlayerJoinedEvent) callback;

            var headComponent = joinEvent.PlayerTransform.gameObject.GetComponentInChildren<HeadAnimation>();
            if (headComponent != null)
            {
                talkingHeads.Add(headComponent);
            }
        }
        
        private void OnPlayerLeave(IEvent<string> callback)
        {
            var leaveEvent = (QuantumPlayerRemovedEvent) callback;

            var headComponent = leaveEvent.PlayerTransform.gameObject.GetComponentInChildren<HeadAnimation>();
            if (headComponent != null)
            {
                talkingHeads.Remove(headComponent);
            }
        }

        private void OnDestroy()
        {
            if (AudioSettingsApplicator.Instance != null)
            {
                AudioSettingsApplicator.Instance.Mixer.SetFloat(DUCK_ATTENUATION_PARAM, 0);
            }
        }
    }
}
