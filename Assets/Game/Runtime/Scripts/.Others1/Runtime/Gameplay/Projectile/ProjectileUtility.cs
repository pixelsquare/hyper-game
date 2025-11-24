using UnityEngine;

namespace Santelmo.Rinsurv
{
    public static class ProjectileUtility
    {
        public static Vector2[] GetConeDirections(Vector2 aimDirection, int amount, float spread)
        {
            var angleInterval = spread / (amount - 1);
            var baseAngle = spread / -2;
            
            var directions = new Vector2[amount];
            for (var i = 0; i < amount; ++i)
            {
                var angle = baseAngle + angleInterval * i;
                directions[i] = Quaternion.AngleAxis(angle, Vector3.forward) * aimDirection;
            }

            return directions;
        }
    }
}
