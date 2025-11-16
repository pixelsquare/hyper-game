using UnityEngine;

namespace Kumu.Extensions
{
    public static class TransformExtensions
    {
        public static Vector3 GetRelativeXzProjection(this Transform referenceTransform, Vector2 vector)
        {
            var forward = Vector3.ProjectOnPlane(referenceTransform.forward, Vector3.up).normalized;
            var right = Vector3.Cross(Vector3.up, forward);
            var result = forward * vector.y + right * vector.x;
            
            return result;
        }
    }
}
