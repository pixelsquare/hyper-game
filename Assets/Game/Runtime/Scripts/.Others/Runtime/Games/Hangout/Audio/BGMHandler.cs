using System;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class BGMHandler : MonoBehaviour
    {
        [Tooltip("Each BGM should have its own source, this allows for continuous playback.")]
        public MusicMapping[] mapping;
        private AudioSource currentSource;

        public void PlayTrack(int index)
        {
            if (index >= mapping.Length)
            {
                Debug.LogWarning($"{gameObject.name} [BGMHandler]: Index {index} does not exist for the current list.");
                return;
            }

            for (int i = 0; i < mapping.Length; i++)
            {
                if (i == index)
                {
                    mapping[i].bgmSource.FadeIn();
                    currentSource = mapping[i].bgmSource.AudioSource;
                }
                else
                {
                    mapping[i].bgmSource.FadeOut();
                }
            }
        }

        private void Awake()
        {
            foreach (var mm in mapping)
            {
                mm.bgmSource.AudioSource.clip = mm.bgmTrack;

                if (mm.isInitialBGM)
                {
                    if (currentSource != null)
                    {
                        currentSource.volume = 0;
                    }

                    mm.bgmSource.AudioSource.volume = 1;
                    currentSource = mm.bgmSource.AudioSource;
                }
                else
                {
                    mm.bgmSource.AudioSource.volume = 0;
                }

                mm.bgmSource.AudioSource.Play();
            }
        }

        [Serializable]
        public class MusicMapping
        {
            public string label;
            public AudioClip bgmTrack;
            public CrossfadeSource bgmSource;
            public bool isInitialBGM;
        }
    }
}
