using UnityEngine;
using DarkTonic.MasterAudio;

namespace Kumu.Kulitan.Common
{
    public class AudioGroupPlayer : MonoBehaviour
    {
        [SoundGroupAttribute, SerializeField] private string soundGroupToPlay;

        public void PlayAudio()
        {
            PlayAudio(soundGroupToPlay);
        }
        
        public void PlayAudio(string soundGroup)
        {
            MasterAudio.PlaySound(soundGroup);
        }
        
        public void PlayAudioFromThisTransform()
        {
            MasterAudio.PlaySound3DAtTransform(soundGroupToPlay, transform);
        }
    }
}
