using System.Collections.Generic;
using System.Linq;
using agora_gaming_rtc;
using Kumu.Kulitan.Common;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class SpatialAudioHandler : MonoBehaviour
    {
        [SerializeField] private SpatialAudioScriptableObject spatialAudioData;
        [SerializeField] private VoiceMuteHandler muteHandler;
        private readonly List<PlayerAudioMap> playerAudioMap = new List<PlayerAudioMap>();
        private List<uint> playerOnGlobalVoice = new List<uint>();

        private Transform localPlayerTransform;
        private HeadAnimation localPlayerHeadAnimation;
        private bool checkSpatialAudio;
    
        private float speakerVolume;
        private bool isVoiceGlobal;

        public void SetLocalPlayer(Transform playerTransform)
        {
            localPlayerTransform = playerTransform;
            localPlayerHeadAnimation = localPlayerTransform.GetComponentInChildren<HeadAnimation>();
            checkSpatialAudio = true;
            
            var f = QuantumRunner.Default.Game.Frames.Verified;

            foreach (var (entity, broadcaster) in f.GetComponentIterator<Broadcaster>())
            {
                var playerRefIndex = f.Get<HangoutPlayer>(entity).player._index;
                var playerAudio = playerAudioMap.FirstOrDefault(x => x.playerRef == playerRefIndex);

                if (playerAudio != null)
                {
                    playerOnGlobalVoice.Add(playerAudio.uID);
                }
            }
            
            muteHandler.OnRequestMuteLocalAudio(true);
        }

        public void OnQuantumInitializePlayer(Transform playerTransform, uint uID, int refId)
        {
            foreach (var map in playerAudioMap.Where(map => map.uID == uID))
            {
                map.OnInitializedByQuantum(uID, playerTransform, refId);
                return;
            }

            var audioMap = new PlayerAudioMap();
            audioMap.OnInitializedByQuantum(uID, playerTransform, refId);
            playerAudioMap.Add(audioMap);
        }

        public void OnPlayerJoinedChannel(uint uID)
        {
            foreach (var map in playerAudioMap.Where(map => map.uID == uID))
            {
                map.OnJoinVoiceChannel(uID);
                return;
            }

            var audioMap = new PlayerAudioMap();
            audioMap.OnJoinVoiceChannel(uID);
            playerAudioMap.Add(audioMap);
        }

        public void OnPlayerLeaveChannel(uint uID)
        {
            var map = playerAudioMap.Find(m => m.uID == uID);
            if (map != null)
            {
                playerAudioMap.Remove(map);
            }
        }

        public void OnVolumeIndicationHandler(AudioVolumeInfo[] speakers, int speakerNumber, int totalVolume)
        {
            foreach (var speaker in speakers)
            {
                if (speaker.uid == 0)
                {
                    localPlayerHeadAnimation.AnimateHead(speaker.volume);
                    return;
                }

                foreach (var map in playerAudioMap.Where(map => map.uID == speaker.uid))
                {
                    if (map.headAnimator != null)
                    {
                        map.headAnimator.AnimateHead(speaker.volume);
                    }
                }
            }
        }

        private float GetGainByPlayerDistance(float distanceToPlayer)
        {
            distanceToPlayer = Mathf.Clamp(distanceToPlayer, spatialAudioData.MinChatProximity, spatialAudioData.MaxChatProximity);

            float gain = (distanceToPlayer - spatialAudioData.MaxChatProximity) / (spatialAudioData.MinChatProximity - spatialAudioData.MaxChatProximity);
            gain *= 100;
            return gain;
        }

        private float GetPanByPlayerDistance(Transform otherUser)
        {
            Vector3 directionToRemotePlayer = otherUser.position - localPlayerTransform.position;

            directionToRemotePlayer.Normalize();

            float pan = Vector3.Dot(localPlayerTransform.right, directionToRemotePlayer);
            return pan;
        }

        private void UpdateSpatialAudio()
        {
            var markedForRemoval = new List<PlayerAudioMap>();
            
            foreach (var audioMap in playerAudioMap)
            {
                if (!audioMap.isReady)
                {
                    continue;
                }
                
                if (audioMap.PlayerTransform == null)
                {
                    markedForRemoval.Add(audioMap);
                    continue;
                }

                var playerDistance =
                    Vector3.Distance(localPlayerTransform.position, audioMap.PlayerTransform.position);

                //This will reset the computation of player distance which will result to global voice in a room
                if (playerOnGlobalVoice.Contains(audioMap.uID))
                {
                    playerDistance = 0;
                }

                var panDistance = GetPanByPlayerDistance(audioMap.PlayerTransform);
                var gainDistance = GetGainByPlayerDistance(playerDistance);

                AudioVideoChatHelper.GetAudioEffectManager().SetRemoteVoicePosition(audioMap.uID, panDistance, gainDistance);
            }
            
            //clean-up
            foreach (var aMap in markedForRemoval)
            {
                playerAudioMap.Remove(aMap);
            }
        }

        private void Awake()
        {
            QuantumEvent.Subscribe<EventOnGlobalChatToggled>(this, OnGlobalChatToggledEvent);
        }

        private void OnGlobalChatToggledEvent(EventOnGlobalChatToggled callback)
        {
            var playerRefIndex = callback.playerRef._index;
            var playerAudio = playerAudioMap.FirstOrDefault(x => x.playerRef == playerRefIndex);

            if (playerAudio == null)
            {
                return;
            }

            if (callback.isEnabled)
            {
                playerOnGlobalVoice.Add(playerAudio.uID);
            }
            else
            {
                playerOnGlobalVoice.Remove(playerAudio.uID);                
            }
        }

        private void Update()
        {
            if (checkSpatialAudio)
            {
                UpdateSpatialAudio();
            }
        }

        protected virtual void OnDestroy()
        {
            QuantumEvent.UnsubscribeListener(this);
        }
        
        private class PlayerAudioMap
        {
            private bool isInitializedByQuantum;
            private bool hasJoinedChannel;

            public uint uID { get; private set; }
            public Transform PlayerTransform { get; private set; }
            public bool isReady => isInitializedByQuantum && hasJoinedChannel;
            public HeadAnimation headAnimator { get; private set; }
            public int playerRef  {get; private set; }

            public void OnInitializedByQuantum(uint uid, Transform playerTransform, int refId)
            {
                uID = uid;
                PlayerTransform = playerTransform;
                headAnimator = PlayerTransform.GetComponentInChildren<HeadAnimation>();
                isInitializedByQuantum = true;
                playerRef = refId;
            }

            public void OnJoinVoiceChannel(uint uid)
            {
                uID = uid;
                hasJoinedChannel = true;
            }
        }
    }
}
