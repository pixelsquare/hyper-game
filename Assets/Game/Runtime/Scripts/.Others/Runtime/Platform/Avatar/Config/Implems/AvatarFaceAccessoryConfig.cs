using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Kumu/Ube/Avatar/Config/FaceAccessory")]
    public class AvatarFaceAccessoryConfig : AvatarApparelConfig, IAvatarItemTintable
    {
        public void TintAvatarItem(IAvatarItemModelHandle colorizer, Color color)
        {
            if (data.itemCategory == "facialhair")
            {
                colorizer.SetColor(color);
            }
            else
            {
                colorizer.SetColor(Color.white);
            }
        }

        public override string GetTypeCode()
        {
            return "FA";
        }
    }
}
