using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class BasePopup : MonoBehaviour, IPopup
    {
        public UniTask<bool> Task { get; protected set; }

        protected bool _isClosed;

        public virtual void OnPopupOpen()
        {
        }

        public virtual void OnPopupClose()
        {
            Cleanup();
            _isClosed = true;
        }

        public void Cleanup()
        {
            Destroy(gameObject);
        }
    }
}
