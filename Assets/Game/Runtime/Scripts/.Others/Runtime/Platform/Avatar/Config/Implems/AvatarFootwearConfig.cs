using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarFootwearConfig : AvatarApparelConfig, IAvatarItemOffsetable
    {
        public Vector3 GetOffset(IAvatarItemModelHandle colorizer)
        {
            if (!colorizer.HasMesh())
            {
                return Vector3.zero;
            }
            
            var bounds = colorizer.MeshBounds();
            
            return Vector3.up * (bounds.extents.y - bounds.center.y);
        }

        public override string GetTypeCode()
        {
            return "FW";
        }
    }
}
