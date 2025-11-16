using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarAnimationEvents : MonoBehaviour
    {
        private Animator animator;
        private float randomSpeed;

        public void BlinkInterval()
        {
            float randomFloat = Random.Range(0.2f, .9f);
            animator.SetFloat("blinkSpeed", randomFloat);
        }

        public void RandomMovement()
        {
            float randomFloat = Random.Range(0f, 1f);
            if (randomFloat > 0.5f && randomFloat < 0.9f)
            {
                randomFloat = 1f;
            }
            else if (randomFloat > 0.1f && randomFloat < 0.4f)
            {
                randomFloat = 0.5f;
            }
            else
            {
                randomFloat = 0f;
            }
            animator.SetFloat("speed", randomFloat);
        }
        private void Start()
        {
            animator = GetComponent<Animator>();
            randomSpeed = Random.Range(15f, 25f);
            RandomMovement();
            BlinkInterval();
        }
    }
}
