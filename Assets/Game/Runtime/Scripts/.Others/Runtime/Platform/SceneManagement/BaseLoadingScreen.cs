using Kumu.Extensions;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class BaseLoadingScreen : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] protected float progress;

        public void Show()
        {
            this.OrderTransform();
            GlobalNotifier.Instance.Trigger(new LoadingScreenStateEvent(LoadingScreenStateEvent.LoadingScreenState.SHOWN));
        }

        public void Hide()
        {
            GlobalNotifier.Instance.Trigger(new LoadingScreenStateEvent(LoadingScreenStateEvent.LoadingScreenState.HIDDEN));
            Destroy(gameObject);
        }

        public virtual void UpdateLoadingProgress(float value)
        {
            progress = value;
        }

        #region ISortableTransform

        public const int DEFAULT_SORT_ORDER = 9998;

        public int Order { get; set; } = DEFAULT_SORT_ORDER;
        
        public Transform GetTransform()
        {
            return transform;
        }

        public Transform GetTransformParent()
        {
            return transform.parent;
        }

        #endregion
    }
}
