using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(Animator))]
    public class LocalCharacterAnimator : MonoBehaviour
    {
        private static readonly int blink = Animator.StringToHash("blinkSpeed");

        [SerializeField] private Vector2 blinkIntervalRange;
        private Animator characterAnimator;
        
        // Animation Event
        public void BlinkInterval()
        {
            var randomFloat = Random.Range(blinkIntervalRange.x, blinkIntervalRange.y);
            characterAnimator.SetFloat(blink, randomFloat);
        }

        private void Awake()
        {
            characterAnimator = GetComponent<Animator>();
        }
    }
}

