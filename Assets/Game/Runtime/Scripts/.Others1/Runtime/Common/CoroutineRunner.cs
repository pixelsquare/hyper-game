using System.Collections;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class CoroutineRunner : MonoBehaviour
    {
        public static void RunCoroutine(IEnumerator coroutine)
        {
            var go = new GameObject("runner");
            go.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            var runner = go.AddComponent<CoroutineRunner>();
            runner.StartCoroutine(runner.UpdateCoroutine(coroutine));
            DontDestroyOnLoad(go);
        }

        private IEnumerator UpdateCoroutine(IEnumerator coroutine)
        {
            while (coroutine.MoveNext())
            {
                yield return coroutine.Current;
            }
            Destroy(gameObject);
        }
    }
}
