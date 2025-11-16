using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Socks")]
    public class AvatarSocksConfig : AvatarApparelConfig
    {
        public override string GetTypeCode()
        {
            return "SK";
        }
    }
}
