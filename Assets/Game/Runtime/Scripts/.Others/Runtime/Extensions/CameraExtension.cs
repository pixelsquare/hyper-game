using UnityEngine;

namespace Kumu.Extensions
{
    /// <summary>
    /// Class to hold all extension methods for cameras
    /// </summary>
    public static class CameraExtension
    {
        /// <summary>
        /// Gets the upper bounds of an orthographic camera.
        /// uses the orthographic size to determine bounds
        /// </summary>
        public static float GetOrthoUpperBoundsY(this Camera camera)
        {
            if (!camera.orthographic)
            {
                Debug.LogError(
                    "Trying to get upper bounds on a non-orthographic camera. This will yield unexpected results");
            }
            return camera.transform.position.y + camera.orthographicSize;
        }
        
        /// <summary>
        /// Gets the lower bounds of an orthographic camera.
        /// uses the orthographic size to determine bounds
        /// </summary>
        public static float GetOrthoLowerBoundsY(this Camera camera)
        {
            if (!camera.orthographic)
            {
                Debug.LogError(
                    "Trying to get lower bounds on a non-orthographic camera. This will yield unexpected results");
            }
            return camera.transform.position.y - camera.orthographicSize;
        }
    }
}
