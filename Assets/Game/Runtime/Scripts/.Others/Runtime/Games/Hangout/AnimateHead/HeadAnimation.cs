using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class HeadAnimation : MonoBehaviour
    {
        private const float MAX_VOLUME = 255;

        [SerializeField] private HeadAnimationScriptableObject headAnimationConfig;
        [SerializeField] private float speakCooldownDuration;

#if !ADDRESSABLES_ENABLED
        [SerializeField] private Transform objectToTween;
#else
        private Transform objectToTween;
#endif

        private Vector3 animationScale;
        private float animationTime;
        private bool isAnimating;
        private float currentVolume;
        private bool didSpeak;
        private float speakCooldownTimer;

        public float CurrentVolume => currentVolume;

#if ADDRESSABLES_ENABLED
        public void InitializeModel(GameObject modelObj)
        {
            // TODO: Hardcoded string!
            objectToTween =
                modelObj.transform.Find(
                    "Avatar_001_Rig_08/Global_CTRL/Body_001_JNT/Hips/Spine/Spine1/Spine2/Neck/Head");
        }
#endif

        public void AnimateHead(float volume)
        {
            currentVolume = volume;
            if (currentVolume > 0)
            {
                if (!isAnimating)
                {
                    isAnimating = true;
                    animationTime = 0;
                    headAnimationConfig.RandomizeDuration();
                }

                didSpeak = true;
                speakCooldownTimer = 0;
            }
            else
            {
                isAnimating = false;
                objectToTween.localScale = Vector3.one;
            }
        }

        private void DoAnimation()
        {
            animationScale.x = headAnimationConfig.XAnimationCurve.Evaluate(animationTime);
            animationScale.y = headAnimationConfig.YAnimationCurve.Evaluate(animationTime);
            animationScale.z = headAnimationConfig.ZAnimationCurver.Evaluate(animationTime);

            objectToTween.localScale = Vector3.one +
                                       (animationScale * headAnimationConfig.AnimationIntensity *
                                        (currentVolume / MAX_VOLUME));
            animationTime += Time.deltaTime / headAnimationConfig.RandomizeDuration();

            if (animationTime > 1f)
            {
                headAnimationConfig.RandomizeDuration();
                animationTime = 0;
                isAnimating = false;
            }
        }
        
        private void Update()
        {
            //reset volume to zero after specified time from last detected broadcast
            if (didSpeak)
            {
                speakCooldownTimer += Time.deltaTime;

                if (speakCooldownTimer >= speakCooldownDuration)
                {
                    didSpeak = false;
                    speakCooldownTimer = 0;
                    currentVolume = 0;
                }
            }
            else if(objectToTween != null)
            {
                isAnimating = false;
                objectToTween.localScale = Vector3.one;
            }
        }

        private void LateUpdate()
        {
            if (!isAnimating || objectToTween == null)
            {
                return;
            }

            DoAnimation();
        }
    }
}
