using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Hair")]
    public class AvatarHairConfig : AvatarApparelConfig, IAvatarItemTintable
    {
        public void TintAvatarItem(IAvatarItemModelHandle colorizer, Color color)
        {
            colorizer.SetColor(color);
        }

        public override string GetTypeCode()
        {
            return "H";
        }
    }
}
