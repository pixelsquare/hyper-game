using UnityEngine;

namespace Santelmo.Rinsurv
{
    public static class CameraExtensions
    {
        public static Vector2 ScreenSizeToWorldSpace2D(this Camera camera)
        {
            var max = camera.ViewportToWorldPoint(Vector3.one);
            var min = camera.ViewportToWorldPoint(Vector3.zero);
            return max - min;
        }

        public static Vector2 ScreenCenterToWorldSpace2D(this Camera camera)
        {
            var center = camera.ViewportToWorldPoint(new Vector3(.5f, .5f, camera.nearClipPlane));
            return center;
        }
    }
}
