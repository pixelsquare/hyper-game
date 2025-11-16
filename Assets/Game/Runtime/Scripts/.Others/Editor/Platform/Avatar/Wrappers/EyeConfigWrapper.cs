using System;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{   
    public class EyeConfigWrapper : AvatarItemConfigEditorWrapper
    {
        private const string FIELD_TEX_BASE_REF = "texBaseRef";
        private const string FIELD_TEX_MASK_REF = "texMaskRef";

        public override AvatarItemConfig CreateConfig(string guid, string filename)
        {
            var configName = AvatarEditorConstants.PATTERN_FILENAME.Match(filename).Value;
            var config = ScriptableObject.CreateInstance<AvatarEyeConfig>();
            
            CreateData(config, guid, configName);
            
            var matName = "Mat_Eyes";
            CreateAssetRefMaterial(config, matName);
            
            CreateAssetRefSprite(config, configName);
            
            CreateAssetRefMesh(config, configName);
            
            CreateAssetRefTextureEye(config, configName, FIELD_TEX_BASE_REF, "base");
            CreateAssetRefTextureEye(config, configName, FIELD_TEX_MASK_REF, "mask");

            return config;
        }
        
        protected override Type GetConfigType()
        {
            return typeof(AvatarEyeConfig);
        }

        private void CreateAssetRefTextureEye(AvatarEyeConfig config, string filename, string fieldName, string affix)
        {
            var filepath = $"{filename}_{affix}";
            
            if (!AvatarConfigWrapperUtil.TryCreateAssetRefTexture(filepath, out var textureRef)
                || !TrySetField(config, textureRef, fieldName))
            {
                var warning = $"No texture found for <color=#ff00ff>{filename}</color>.";
                Debug.LogWarning(warning);
            }
        }
    }
}
