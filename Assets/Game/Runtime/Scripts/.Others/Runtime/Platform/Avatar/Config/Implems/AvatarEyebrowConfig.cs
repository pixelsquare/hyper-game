using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Eyebrow")]
    public class AvatarEyebrowConfig : AvatarFaceConfig, IAvatarItemTintable
    {
        public void TintAvatarItem(IAvatarItemModelHandle colorizer, Color color)
        {
            colorizer.SetColor(color);
        }

        public override string GetTypeCode()
        {
            return "EB";
        }
    }
}
