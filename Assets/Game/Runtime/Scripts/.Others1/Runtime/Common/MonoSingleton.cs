using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Tooltip("If true, will be considered as a DontDestroyOnLoad object")]
        [SerializeField] private bool _isPersistent;
        
        public static T Instance { get; private set; }
                
        protected virtual void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this as T;

                if (_isPersistent)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
