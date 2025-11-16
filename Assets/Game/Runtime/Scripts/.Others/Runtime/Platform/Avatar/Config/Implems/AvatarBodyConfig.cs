using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/Body")]
    public class AvatarBodyConfig : AvatarFaceConfig, IAvatarItemTintable, IAvatarItemSameTintable
    {
        public override string GetTypeCode()
        {
            return "B";
        }

        public void TintAvatarItem(IAvatarItemModelHandle colorizer, Color color)
        {
            colorizer.SetColor(color);
        }

        public void TintAvatarItem(IAvatarItemModelHandle source, IAvatarItemModelHandle target, Color color)
        {
            target.SetColor(color);
        }

        public string GetTargetTypeCode()
        {
            return "N";
        }
    }
}
