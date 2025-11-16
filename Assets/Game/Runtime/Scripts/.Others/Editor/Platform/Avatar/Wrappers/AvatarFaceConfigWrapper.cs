using System;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarFaceConfigWrapper<T> : AvatarItemConfigEditorWrapper
        where T : AvatarFaceConfig
    {

        protected abstract string GetMaterialName();

        public override AvatarItemConfig CreateConfig(string guid, string filename)
        {   
            var config = ScriptableObject.CreateInstance<T>();
            
            CreateData(config, guid, filename);
            
            var matName = GetMaterialName();
            CreateAssetRefMaterial(config, matName);
            
            CreateAssetRefSprite(config, filename);
            
            CreateAssetRefTexture(config, filename);

            return config;
        }
        
        protected override Type GetConfigType()
        {
            return typeof(T);
        }
    }

    public class BodyConfigWrapper : AvatarFaceConfigWrapper<AvatarBodyConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_Body";
        }
    }

    public class EyebrowConfigWrapper : AvatarFaceConfigWrapper<AvatarEyebrowConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_Brows";
        }
    }

    public class MouthConfigWrapper : AvatarFaceConfigWrapper<AvatarMouthConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_Mouth";
        }
    }


    public class SkinDesignConfigWrapper : AvatarFaceConfigWrapper<AvatarSkinDesignConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_SkinDesign";
        }
    }

    public class SkinMarkConfigWrapper : AvatarFaceConfigWrapper<AvatarSkinMarkConfig>
    {
        protected override string GetMaterialName()
        {
            return "Mat_SkinMark";
        }
    }
}
