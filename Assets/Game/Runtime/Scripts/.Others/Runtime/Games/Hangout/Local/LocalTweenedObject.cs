using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class LocalTweenedObject : MonoBehaviour
    {
        [SerializeField] private Transform startPositionTransform = null;

        [SerializeField] private Transform endPositionTransform = null;

        [SerializeField] private float speed = 0f;

        private Transform targetPositionTransform = null;
        
        void Awake()
        {
            targetPositionTransform = startPositionTransform;
        }

        void Update()
        {
            var step = speed * Time.deltaTime;

            if (targetPositionTransform != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPositionTransform.position, step);
            }
        }

        public void TweenToStartPosition()
        {
            if (startPositionTransform != null)
            {
                targetPositionTransform = startPositionTransform;
            }
        }
        public void TweenToEndPosition()
        {
            if (endPositionTransform != null)
            {
                targetPositionTransform = endPositionTransform;
            }
        }
    }
}
