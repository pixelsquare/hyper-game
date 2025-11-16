using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Mouth")]
    public class AvatarMouthConfig : AvatarFaceConfig
    {
        public override string GetTypeCode()
        {
            return "M";
        }
    }
}
