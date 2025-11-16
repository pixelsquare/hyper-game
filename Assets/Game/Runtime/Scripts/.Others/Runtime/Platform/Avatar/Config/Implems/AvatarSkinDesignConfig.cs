using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/SkinDesign")]
    public class AvatarSkinDesignConfig : AvatarFaceConfig
    {
        public override string GetTypeCode()
        {
            return "SD";
        }
    }
}
