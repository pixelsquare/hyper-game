using System.Collections;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class CoroutineRunner : MonoBehaviour
    {
        public static bool IsBusy { get; private set; }
        
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
            IsBusy = true;
            while (coroutine.MoveNext())
            {
                yield return coroutine.Current;
            }
            IsBusy = false;
            Destroy(gameObject);
        }
    }
}
