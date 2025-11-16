using System.Collections.Generic;
using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Multiplayer;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class TestRoomsRemover : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }
        private const string DEFAULT_ROOM_LAYOUT_CONFIGS_PATH = 
            "Assets/_Source/Content/ScriptableObjects/Portal/RoomLayout/DefaultRoomLayoutConfigs.asset";
        private const string ADDRESSABLE_ASSET_SETTINGS = 
            "Assets/AddressableAssetsData/AddressableAssetSettings.asset";

        public void OnPreprocessBuild(BuildReport report)
        {
            var identifier = Application.identifier;
            if (identifier.Contains("dev") || identifier.Contains("stg"))
            {
                return;
            }

            var roomLayoutConfigs = AssetDatabase.LoadAssetAtPath<RoomLayoutConfigs>(DEFAULT_ROOM_LAYOUT_CONFIGS_PATH);
            var configs = roomLayoutConfigs.LayoutConfigs.ToList();

            roomLayoutConfigs.LayoutConfigs = GetFilteredLayoutConfig(configs);
            RemoveTestRoomFromAddressables(configs);
            EditorUtility.SetDirty(roomLayoutConfigs);
            AssetDatabase.SaveAssets();
        }

        private RoomLayoutConfig[] GetFilteredLayoutConfig(List<RoomLayoutConfig> configs)
        {
            configs.RemoveAll(config => config.IsTestRoom);
            return configs.ToArray();
        }

        private void RemoveTestRoomFromAddressables(List<RoomLayoutConfig> configs)
        {
            var settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(ADDRESSABLE_ASSET_SETTINGS);

            var layoutsToRemove = configs.FindAll(config => config.IsTestRoom);
            foreach (var layout in layoutsToRemove)
            {
                var parsedSceneName = layout.LevelConfig.SceneToLoad.Substring(0,
                    layout.LevelConfig.SceneToLoad.IndexOf("-"));
                var adrGrpName = $"Scene{parsedSceneName}";
                var group = settings.FindGroup(adrGrpName);
                if (!group)
                {
                    "Attempting to remove a non-existent addressable group".LogWarning();
                    continue;
                }
                settings.RemoveGroup(group);
            }
        }
    }
}
