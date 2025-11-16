using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/HandAccessory")]
    public class AvatarHandAccessoryConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "HD";
        }
    }
}
