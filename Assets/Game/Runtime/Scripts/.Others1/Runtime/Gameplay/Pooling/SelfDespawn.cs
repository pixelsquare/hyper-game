using UnityEngine;

namespace Santelmo.Rinsurv
{
    //class for self-clearing objects like VFX
    public class SelfDespawn : MonoBehaviour
    {
        [SerializeField] private float _despawnTime;
        
        private float _interval;
        private Transform _returnParent;

        public void SetReturnParent(Transform transform)
        {
            _returnParent = transform;
        }
        
        private void Awake()
        {
            _interval = _despawnTime;
        }

        private void Update()
        {
            if (gameObject.activeSelf)
            {
                if (_interval > 0)
                {
                    _interval -= Time.deltaTime;
                }
                else
                {
                    _interval = _despawnTime;
                    transform.parent = _returnParent;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
