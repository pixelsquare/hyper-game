using UnityEngine;

namespace Santelmo.Rinsurv
{
    public static class GameplayAnimationUtility
    {
        private static readonly float[] _angleThresholds =
        {
            //0
            22.5f, //45 
            67.5f, //90 
            112.5f, //135
            157.5f, //180
        };
        
        public static int DirectionToIndex(Vector2 direction, float[] angleThresholds)
        {
            var uangle = Vector2.Angle(Vector2.up, direction);
            var isFlipped = direction.x < 0f;

            var idx = 0;
            foreach (var threshold in angleThresholds)
            {
                if (uangle > threshold)
                {
                    ++idx;
                }
                else
                {
                    break;
                }
            }

            if (!isFlipped)
            {                
                idx = angleThresholds.Length * 2 - idx;
            }

            return idx;
        }
        
        public static int DirectionToIndex(Vector2 direction)
        {
            return DirectionToIndex(direction, _angleThresholds);
        }
        
        public static float Angle360(Vector2 p1, Vector2 p2, Vector2 o = default(Vector2))
        {
            Vector2 v1, v2;
            if (o == default(Vector2))
            {
                v1 = p1.normalized;
                v2 = p2.normalized;
            }
            else
            {
                v1 = (p1 - o).normalized;
                v2 = (p2 - o).normalized;
            }
            float angle = Vector2.Angle(v1, v2);
            return Mathf.Sign(Vector3.Cross(v1, v2).z) < 0 ? (360 - angle) % 360 : angle;
        }
    }
}
