using System;
using Kumu.Extensions;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class BasePopup : MonoBehaviour, IPopup
    {
        public int Priority { get; set; }

        public Action OnOpened { get; set; }
        public Action OnClosed { get; set; }

        public void Open()
        {
            gameObject.SetActive(true);
            this.OrderTransform();
            // Play popup open animation
            OnOpened?.Invoke();
        }

        public void Close()
        {
            // Play popup close animation
            // Disable/Destroy gameobject after popup closed
            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            Destroy(gameObject);
        }

        #region ISortableTransforms

        public const int DEFAULT_SORT_ORDER = 9999;

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
