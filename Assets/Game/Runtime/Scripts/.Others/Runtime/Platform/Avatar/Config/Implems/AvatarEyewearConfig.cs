using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Eyewear")]
    public class AvatarEyewearConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "EW";
        }
    }
}
