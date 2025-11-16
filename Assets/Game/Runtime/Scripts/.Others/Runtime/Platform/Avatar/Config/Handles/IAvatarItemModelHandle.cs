using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public interface IAvatarItemModelHandle
    {
        void SetMaterial(Material sourceMaterial);
        void SetTexture(params Texture[] textures);
        void SetColor(Color color);
        void SetMesh(Mesh mesh);
        bool HasMesh();
        Bounds MeshBounds();
        void SetMeshEnabled(bool isEnabled);
    }
}
