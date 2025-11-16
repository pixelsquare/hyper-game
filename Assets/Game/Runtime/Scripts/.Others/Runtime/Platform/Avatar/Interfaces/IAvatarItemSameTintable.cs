using Kumu.Kulitan.Avatar;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public interface IAvatarItemSameTintable
    {
        public void TintAvatarItem(IAvatarItemModelHandle source, IAvatarItemModelHandle target, Color color);
        public string GetTargetTypeCode();
    }
}
