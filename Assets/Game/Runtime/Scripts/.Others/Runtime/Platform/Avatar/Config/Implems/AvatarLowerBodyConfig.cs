using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/LowerBody")]
    public class AvatarLowerBodyConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "LB";
        }
    }
}
