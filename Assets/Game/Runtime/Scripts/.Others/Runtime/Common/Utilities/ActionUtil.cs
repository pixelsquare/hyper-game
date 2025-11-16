using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public static class ActionUtil
    {
        public static Coroutine DelayFrame(this MonoBehaviour monoBehaviour, UnityAction action, int frames = 1)
        {
            var ienumeratorTask = DelayFrame(action, frames);
            return monoBehaviour.StartCoroutine(ienumeratorTask);
        }

        public static Coroutine DelayEndOfFrame(this MonoBehaviour monoBehaviour, UnityAction action)
        {
            var ienumeratorTask = DelayEndOfFrame(action);
            return monoBehaviour.StartCoroutine(ienumeratorTask);
        }

        private static IEnumerator DelayFrame(UnityAction action, int frames)
        {
            while(frames-- > 0)
            {
                yield return null;
            }

            action?.Invoke();
        }

        private static IEnumerator DelayEndOfFrame(UnityAction action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }
    }
}
