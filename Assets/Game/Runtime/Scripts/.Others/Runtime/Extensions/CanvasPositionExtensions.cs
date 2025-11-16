using UnityEngine;

namespace Kumu.Extensions
{
    /// <summary>
    /// Class to hold all extension methods for canvas positions
    /// </summary>
    public static class CanvasPositionExtensions
    {
        /// <summary>
        /// Converts world position to canvas position.
        /// This also accounts for canvas that uses `CanvasScaler`
        /// </summary>
        public static Vector3 WorldToCanvasPosition(this RectTransform transform, Vector3 worldPosition, Camera camera)
        {
            var viewportPosition = camera.WorldToViewportPoint(worldPosition);
            return transform.ViewportToCanvasPosition(viewportPosition);
        }

        public static Vector3 ViewportToCanvasPosition(this RectTransform transform, Vector3 viewportPosition)
        {
            var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
            return Vector3.Scale(centerBasedViewPortPosition, transform.sizeDelta);
        }
    }
}
