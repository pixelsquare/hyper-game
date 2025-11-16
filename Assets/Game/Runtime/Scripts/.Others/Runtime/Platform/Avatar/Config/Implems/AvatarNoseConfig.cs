using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Nose")]
    public class AvatarNoseConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "N";
        }
    }
}
