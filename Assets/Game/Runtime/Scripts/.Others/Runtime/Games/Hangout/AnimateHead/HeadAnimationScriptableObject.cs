using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Head Animation Config")]
    public class HeadAnimationScriptableObject : ScriptableObject
    {
        [SerializeField] private AnimationCurve xAnimationCurve;
        [SerializeField] private AnimationCurve yAnimationCurve;
        [SerializeField] private AnimationCurve zAnimationCurve;
        [SerializeField] private float minDuration;
        [SerializeField] private float maxDuration;
        [SerializeField] private float animationIntensity;

        public AnimationCurve XAnimationCurve => xAnimationCurve;
        public AnimationCurve YAnimationCurve => yAnimationCurve;
        public AnimationCurve ZAnimationCurver => zAnimationCurve;
        public float MinDuration => minDuration;
        public float MaxDuration => maxDuration;
        public float AnimationIntensity => animationIntensity;

        public float RandomizeDuration()
        {
            var randomDuration = Random.Range(MinDuration, MaxDuration);
            return randomDuration;
        }
    }
}
