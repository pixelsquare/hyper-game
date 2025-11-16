using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/FullBody")]
    public class AvatarFullBodyConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "FB";
        }
    }
}
