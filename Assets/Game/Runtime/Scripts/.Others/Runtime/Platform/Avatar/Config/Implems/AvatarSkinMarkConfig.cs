using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/SkinMark")]
    public class AvatarSkinMarkConfig : AvatarFaceConfig
    {
        public override string GetTypeCode()
        {
            return "SM";
        }
    }
}
