using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/UpperBody")]
    public class AvatarUpperBodyConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "UB";
        }
    }
}
