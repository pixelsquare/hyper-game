using System.Collections.Generic;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    [RequireComponent(typeof(BoxCollider))]
    public class LocalTweenedObjectTrigger : MonoBehaviour
    {
        [SerializeField] List<LocalTweenedObject> tweenedObjects = null;

        private void LateUpdate()
        {
            foreach (LocalTweenedObject tweenedObject in tweenedObjects)
            {
                tweenedObject.TweenToStartPosition();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == gameObject.layer)
            {
                foreach (LocalTweenedObject tweenedObject in tweenedObjects)
                {
                    tweenedObject.TweenToEndPosition();
                }
            }
        }
    }
}
