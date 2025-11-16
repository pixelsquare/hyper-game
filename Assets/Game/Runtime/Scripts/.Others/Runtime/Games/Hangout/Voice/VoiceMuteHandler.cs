using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Hangout
{
    public class VoiceMuteHandler : MonoBehaviour
    {
        [SerializeField] private Button muteButton;
        [SerializeField] private Sprite iconMute;
        [SerializeField] private Sprite iconMic;
        private bool isMuted;

        public bool IsMuted => isMuted;

        /// <summary>
        /// Checks if the MuteLocalAudioStream was succesfully triggered
        /// If request was rejected it will not change UI Button image
        /// <param name="willMute">True if local audio is muted Otherwise false.</param>
        /// </summary>
        public void OnRequestMuteLocalAudio(bool willMute)
        {
            var muteResult = AudioVideoChatHelper.GetRTCEngine().MuteLocalAudioStream(willMute);
            if (muteResult == 0)
            {
                isMuted = willMute;
                muteButton.image.sprite = willMute ? iconMute : iconMic;
                GlobalNotifier.Instance.Trigger(new ToggleMuteEvent(isMuted));
            }
            else if (muteResult < 0)
            {
                Debug.LogError("Mute toggle request failed. Button state will not be changed");
            }
        }

        public void ToggleMuteUnmuteLocalPlayer()
        {
            OnRequestMuteLocalAudio(!isMuted);
        }

        private void Start()
        {
            //Must set mute to true before JoinChannel triggers
            isMuted = true;
            OnRequestMuteLocalAudio(isMuted);
        }
    }
}
