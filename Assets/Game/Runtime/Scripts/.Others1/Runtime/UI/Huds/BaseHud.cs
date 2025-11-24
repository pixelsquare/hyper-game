using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class BaseHud : MonoBehaviour
    {
        public UniTask Task { get; protected set; }

        protected bool _isClosed;

        public virtual void Cleanup()
        {
            _isClosed = true;
            Destroy(gameObject);
        }
    }
}
