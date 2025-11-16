using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/UpperAccesory")]
    public class AvatarUpperAccessoryConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "UA";
        }
    }
}
