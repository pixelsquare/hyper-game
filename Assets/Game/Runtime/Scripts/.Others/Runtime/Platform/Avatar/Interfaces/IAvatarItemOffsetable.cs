using Kumu.Kulitan.Avatar;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public interface IAvatarItemOffsetable
    {
        public Vector3 GetOffset(IAvatarItemModelHandle colorizer);
    }
}
