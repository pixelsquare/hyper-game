using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public interface IMaterialInstanceProvider
    {
        public Material ProvideMaterialInstance(Material sourceMaterial);
        public bool ReleaseMaterialInstance();
    }
}
