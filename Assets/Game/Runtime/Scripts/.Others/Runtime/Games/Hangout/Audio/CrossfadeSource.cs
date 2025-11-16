using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class CrossfadeSource : MonoBehaviour
    {
        [SerializeField] private float fadeIncrement;
        [SerializeField] private AudioSource mySource;
        private int fadeDirection;

        public AudioSource AudioSource => mySource;

        public void FadeIn()
        {
            fadeDirection = 1;
        }

        public void FadeOut()
        {
            fadeDirection = -1;
        }

        private void Update()
        {
            if (fadeDirection != 0)
            {
                mySource.volume += fadeIncrement * fadeDirection;

                if (mySource.volume <= 0 && fadeDirection < 0)
                {
                    mySource.volume = 0;
                    fadeDirection = 0;
                }
                else if (mySource.volume >= 1 && fadeDirection > 0)
                {
                    mySource.volume = 1;
                    fadeDirection = 0;
                }
            }
        }
    }
}
