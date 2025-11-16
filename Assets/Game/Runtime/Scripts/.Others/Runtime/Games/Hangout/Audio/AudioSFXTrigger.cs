using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class AudioSFXTrigger : MonoBehaviour
    {
        [SerializeField] private AudioUISFXConfig.SFXClipCode triggerCode;
        private AudioUISFXPlayer sfxPlayer;

        public void PlayUISFX()
        {
            FindPlayer();

            if (sfxPlayer != null)
            {
                sfxPlayer.PlayUISFX(triggerCode);
            }
            else
            {
                Debug.LogWarning("WARNING: Audio player script does not exist in open scenes");
            }
        }

        private AudioUISFXPlayer FindPlayer()
        {
            if (sfxPlayer == null)
            {
                sfxPlayer = GameObject.FindObjectOfType<AudioUISFXPlayer>();
            }

            return sfxPlayer;
        }
    }
}
