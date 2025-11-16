using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public class MaterialInstanceProvider : IMaterialInstanceProvider
    {
        private Material cachedMaterialInstance;

        public bool HasCachedMaterial => cachedMaterialInstance != null;

        public Material ProvideMaterialInstance(Material sourceMaterial)
        {
            if (HasCachedMaterial)
            {
                return cachedMaterialInstance;
            }
            
            var mat = new Material(sourceMaterial.shader);
            mat.CopyPropertiesFromMaterial(sourceMaterial);

            cachedMaterialInstance = mat;

            return cachedMaterialInstance;
        }

        public bool ReleaseMaterialInstance()
        {
            if (!HasCachedMaterial)
            {
                return false;
            }
            
            Object.Destroy(cachedMaterialInstance);

            return true;
        }
    }
}
