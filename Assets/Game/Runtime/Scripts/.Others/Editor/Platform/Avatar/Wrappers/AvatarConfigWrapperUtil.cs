using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    public static class AvatarConfigWrapperUtil
    {
        public static bool TryCreateAssetRefMesh(string filename, out AssetReference meshRef)
        {
            var directory = AvatarEditorConstants.DIRECTORY_ITEM_MESH;

            if (!TryGetGuids(filename, directory, out var guid))
            {
                meshRef = null;
                return false;
            }
            
            meshRef = new AssetReference(guid);

            var path = AssetDatabase.GUIDToAssetPath(guid);
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            foreach (var asset in assets)
            {
                if (asset is not Mesh)
                {
                    continue;
                }
                
                meshRef.SubObjectName = asset.name;
                break;
            }
            
            return true;
        }
        
        public static bool TryCreateAssetRefMaterial(string matName, out AssetReference materialRef)
        {
            var directory = AvatarEditorConstants.DIRECTORY_ITEM_MATS;

            if (!TryGetGuids<Material>(matName, directory, out var guid))
            {
                materialRef = null;
                return false;   
            }
            
            materialRef = new AssetReference(guid);
            return true;
        }

        public static bool TryCreateAssetRefTexture(string filename, out AssetReferenceTexture textureRef)
        {
            var directory = AvatarEditorConstants.DIRECTORY_ITEM_TEX;
            
            if (!TryGetGuids<Texture>(filename, directory, out var guid))
            {
                textureRef = null;
                return false;
            }
            
            textureRef = new AssetReferenceTexture(guid);
            return true;
        }

        public static bool TryCreateAssetRefSprite(string filename, out AssetReferenceSprite spriteRef)
        {
            var directory = AvatarEditorConstants.DIRECTORY_ITEM_ICONS;
            if (!TryGetGuids<Sprite>($"{filename}_preview", directory, out var guid))
            {
                spriteRef = null;
                return false;
            }
            
            spriteRef = new AssetReferenceSprite(guid);
            
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            foreach (var asset in assets)
            {
                if (asset is not Sprite)
                {
                    continue;
                }
                
                spriteRef.SubObjectName = asset.name;
                break;
            }

            return true;
        }

        private static bool TryGetGuids<T2>(string filename, string directory, out string guid) where T2 : Object
        {
            var query = $"t:{typeof(T2).Name} {filename}";
            
            var directories = new[] { directory };
            var guids = AssetDatabase.FindAssets(query, directories);

            if (guids.Length == 0)
            {
                guid = string.Empty;
                return false;
            }
            
            if (guids.Length > 1)
            {
                foreach (var id in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(id);
                    var asset = AssetDatabase.LoadAssetAtPath<T2>(path);
                    if (asset.name == filename)
                    {
                        guid = id;
                        return true;
                    }
                }

                guid = string.Empty;
                return false;
            }            
            
            guid = guids[0];
            return true;
        }

        private static bool TryGetGuids(string query, string directory, out string guid)
        {
            var directories = new[] { directory };
            var guids = AssetDatabase.FindAssets(query, directories);

            if (guids.Length == 0)
            {
                guid = string.Empty;
                return false;
            }
            
            guid = guids[0];
            return true;
        }
    }
}
