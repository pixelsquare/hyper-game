using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarItemModelHandleBase : MonoBehaviour, IAvatarItemModelHandle
    {
        public abstract void SetMaterial(Material sourceMaterial);
        public abstract void SetTexture(params Texture[] textures);
        public abstract void SetColor(Color color);
        public abstract void SetMesh(Mesh mesh);
        public abstract bool HasMesh();
        public abstract Bounds MeshBounds();
        public abstract void SetMeshEnabled(bool isEnabled);
    }
}
