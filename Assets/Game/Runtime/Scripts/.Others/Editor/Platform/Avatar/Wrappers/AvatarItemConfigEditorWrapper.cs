using System;
using System.Reflection;
using Kumu.Kulitan.Backend;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    public abstract class AvatarItemConfigEditorWrapper
    {
        protected abstract Type GetConfigType();

        public abstract AvatarItemConfig CreateConfig(string guid, string filename);

        protected void CreateData<T>(T config, string guid, string filename) where T : AvatarItemConfig
        {
            var info = filename.Split("_");

            var data = new AvatarItemData
            {
                itemId = guid,
                itemName = filename,
                cost = new Currency() { code = Currency.UBE_COI, amount = 0 },
                markUpDownCost = 0,
                itemCategory = info[1].ToLower()
            };

            if (!TrySetField(config, data, AvatarEditorConstants.FIELD_DATA))
            {
                var warning = $"No AvatarItemData set for <color=#00ffff>{filename}</color>.";
                Debug.LogWarning(warning);
            }
        }

        protected void CreateAssetRefMesh<T>(T config, string filename) where T : AvatarItemConfig
        {
            var meshName = AvatarEditorConstants.PATTERN_FILENAME.Match(filename).Value;
            
            if (!AvatarConfigWrapperUtil.TryCreateAssetRefMesh(meshName, out var materialRef)
                || !TrySetField(config, materialRef, AvatarEditorConstants.FIELD_MESH_REF))
            {
                var warning = $"Mesh <color=#00ffff>{filename}</color> not found.";
                Debug.LogWarning(warning);
            }
        }

        protected void CreateAssetRefMaterial<T>(T config, string matName) where T : AvatarItemConfig
        {
            if (!AvatarConfigWrapperUtil.TryCreateAssetRefMaterial(matName, out var materialRef)
                || !TrySetField(config, materialRef, AvatarEditorConstants.FIELD_MATERIAL_REF))
            {
                var warning = $"Material <color=#00ffff>{matName}</color> not found.";
                Debug.LogWarning(warning);
            }
        }

        protected void CreateAssetRefSprite<T>(T config, string filename) where T : AvatarItemConfig
        {
            if (!AvatarConfigWrapperUtil.TryCreateAssetRefSprite(filename, out var spriteRef)
                || !TrySetField(config, spriteRef, AvatarEditorConstants.FIELD_SPRITE_REF))
            {
                var warning = $"No sprite found for <color=#ff00ff>{filename}</color>.";
                Debug.LogWarning(warning);
            }
        }

        protected void CreateAssetRefTexture<T>(T config, string filename) where T : AvatarItemConfig
        {
            if (!AvatarConfigWrapperUtil.TryCreateAssetRefTexture(filename, out var textureRef)
                || !TrySetField(config, textureRef, AvatarEditorConstants.FIELD_TEXTURE_REF))
            {
                var warning = $"No texture found for <color=#ff00ff>{filename}</color>.";
                Debug.LogWarning(warning);
            }
        }

        protected bool TrySetField<T1, T2>(T1 config, T2 value, string fieldName)
            where T1 : AvatarItemConfig
        {
            var field = GetConfigType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (field != null)
            {
                field.SetValue(config, value);
                return true;
            }

            return false;
        }
    }
}
