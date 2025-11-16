using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Headwear")]
    public class AvatarHeadwearConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "HW";
        }
    }
}
