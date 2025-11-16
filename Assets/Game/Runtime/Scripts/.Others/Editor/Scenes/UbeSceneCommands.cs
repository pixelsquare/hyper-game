using System.Collections.Generic;
using System.IO;
using Kumu.Extensions;
using Quantum.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Kumu.Kulitan.EditorTools
{
    public static class UbeSceneCommands
    {
        private static readonly string PATH_SCENES = "Assets/_Source/Content/Scenes";
        private static readonly string PATH_PLATFORM_SCENES = Path.Combine(PATH_SCENES, "Platform");

        private static readonly string PATH_PLATFORM_BOOTSTRAP = Path.Combine(PATH_PLATFORM_SCENES, "Bootstrap.unity");
        private static readonly string PATH_PLATFORM_AVATAR_CUSTOMIZATION = Path.Combine(PATH_PLATFORM_SCENES, "AvatarCustomization.unity");

        private static readonly string PATH_HANGOUT_SCENES = Path.Combine(PATH_SCENES, "Quantum", "Hangout");

        [MenuItem("Tools/Refresh Ube Scene Addressables")]
        private static void RefreshSceneAddressables()
        {
            var guids = AssetDatabase.FindAssets("t:scene", new[] { PATH_HANGOUT_SCENES });
            foreach (var guid in guids)
            {
                CreateOrFindAddrGroup(guid);
            }
        }

        private static void CreateOrFindAddrGroup(string guid)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            var sceneName = asset.name.Substring(0, asset.name.IndexOf("-"));
            var addrGroupName = "Scene" + sceneName;
            var addrGroup = settings.FindGroup(addrGroupName);
            if (addrGroup != null)
            {
               ClearAddressablesGroup(addrGroup);
            }
            else
            {
                $"{addrGroup} not found!".LogError();
                return;
            }
            var e = settings.CreateOrMoveEntry(guid, addrGroup, false, false);
            e.SetLabel("default", true, false, false);
            e.SetAddress(sceneName+"-Quantum", false);
            var entriesAdded = new List<AddressableAssetEntry> {e};
            addrGroup.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true);
            EditorUtility.SetDirty(asset);
        }
        
        private static void ClearAddressablesGroup(AddressableAssetGroup assetGroup)
        {
            var entries = new List<AddressableAssetEntry>(assetGroup.entries);
            foreach (var entry in entries)
            {
                assetGroup.RemoveAssetEntry(entry);
            }
        }

        [MenuItem("Tools/Ube Scenes/Bootstrap %g", false, 0)]
        private static void OpenBootstrapScene()
        {
            OpenScene(PATH_PLATFORM_BOOTSTRAP);
        }

        [MenuItem("Tools/Ube Scenes/Avatar Customization", false, 1)]
        private static void OpenAvatarCustomizationScene()
        {
            OpenScene(PATH_PLATFORM_AVATAR_CUSTOMIZATION);
        }

        private static void OpenScene(string path)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            EditorApplication.EnterPlaymode();
        }
    }
}
