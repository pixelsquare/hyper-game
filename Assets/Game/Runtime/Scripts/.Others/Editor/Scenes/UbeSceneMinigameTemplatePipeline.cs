#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Kumu.Kulitan.Multiplayer;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor.SceneTemplate;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.EditorTools
{
    public class UbeSceneMinigameTemplatePipeline : ISceneTemplatePipeline
    {
        private const string SCENE_PATH = "Assets/_Source/Content/Scenes/Quantum/Minigame/";
        
        private const string MAP_DATA_PATH = "Assets/_Source/Content/QuantumAssets/Minigame/Levels";
        
        private const string LEVEL_CONFIG_PATH = "Assets/_Source/Content/ScriptableObjects/Portal/HangoutLevels/";
        private const string LEVEL_CONFIG_TEMPLATE = "_LevelConfigTemplate.asset";

        private const string DEFAULT_ROOM_LAYOUT_CONFIGS_PATH = 
            "Assets/_Source/Content/ScriptableObjects/Portal/RoomLayout/DefaultRoomLayoutConfigs.asset";
        private const string ROOM_LAYOUT_CONFIG_PATH =
            "Assets/_Source/Content/ScriptableObjects/Portal/RoomLayout/LayoutConfigs/";
        private const string ROOM_LAYOUT_TEMPLATE = "_RoomLayoutTemplate.asset";

        private const string ADDRESSABLE_ASSET_SETTINGS = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        
        private const string SCRIPTABLE_EXTENSION = ".asset";
        
        public bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
        {
            return true;
        }

        public void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
        {
        }

        public void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName)
        {
            Debug.Log($"Setting up {sceneName}");
            if (!SetupSceneForQuantum(scene))
            {
                Debug.LogError("Terminating hangout scene template setup");
                return;
            }
            Debug.Log($"Setup successful {scene.name}");
        }

        private static bool SetupSceneForQuantum(Scene scene)
        {
            EditorSceneManager.SaveScene(scene);

            // Filter -Quantum file name
            var assetName = scene.name;
            if (assetName.Contains("-Quantum"))
            {
                assetName = assetName.Replace("-Quantum", "");
            }
            
            if (!CreateAssetMap(scene, assetName, out var mapAsset))
            {
                return false;
            }

            if (!CreateLevelConfig(scene, assetName, mapAsset, out var levelConfig))
            {
                return false;
            }

            if (!CreateRoomLayoutConfig(assetName, levelConfig))
            {
                return false;
            }

            if (!AddNewSceneToAddressableGroups(scene, assetName))
            {
                return false;
            }

            return true;
        }

        private static bool CreateAssetMap(Scene scene, string assetName, out MapAsset mapAsset)
        {
            mapAsset = null;
            var mapDataObjects = UbeEditorUtilities.FindAllObjectsOfTypeInScene<MapData>(scene);
            if (!mapDataObjects.Any())
            {
                Debug.LogError("Scene doesn't contain any gameobjects with `MapData`");
                return false;
            }
            if (mapDataObjects.Count() > 1)
            {
                Debug.LogWarning("Scene contains multiple gameobjects with `MapData` component, this may yield unexpected results");
            }
            
            var mapData = mapDataObjects.First();
            mapAsset = ScriptableObject.CreateInstance<MapAsset>();
            
            var assetPath = AssetDatabase.CreateFolder(MAP_DATA_PATH, assetName);
            
            if (assetPath == string.Empty)
            {
                Debug.LogError("Folder creation failed.");
                return false;
            }
            
            AssetDatabase.CreateAsset(mapAsset, AssetDatabase.GUIDToAssetPath(assetPath) + "/" + assetName + SCRIPTABLE_EXTENSION);
            AssetDatabase.SaveAssets();
            
            mapData.Asset = mapAsset;
            MapDataBaker.BakeMapData(mapData, true);
            EditorSceneManager.SaveScene(scene);

            return true;
        }

        private static bool CreateLevelConfig(Scene scene, string assetName, MapAsset mapAsset, out LevelConfigScriptableObject levelConfig)
        {
            levelConfig = null;
            var newLevelConfigPath = LEVEL_CONFIG_PATH + assetName + "LevelConfig" + SCRIPTABLE_EXTENSION;
            var template =
                AssetDatabase.LoadAssetAtPath<LevelConfigScriptableObject>(LEVEL_CONFIG_PATH + LEVEL_CONFIG_TEMPLATE);
            if (template == null)
            {
                Debug.LogError("LevelConfig template cannot be found!");
                return false;
            }
            
            levelConfig = Object.Instantiate(template);
            levelConfig.EditorSetLevelConfig(scene.name, mapAsset, true);
            AssetDatabase.CreateAsset(levelConfig, newLevelConfigPath);
            AssetDatabase.SaveAssets();

            return true;
        }

        private static bool CreateRoomLayoutConfig(string assetName, LevelConfigScriptableObject levelConfig)
        {
            var newRoomLayoutConfigPath = $"{ROOM_LAYOUT_CONFIG_PATH}{assetName}RoomLayout{SCRIPTABLE_EXTENSION}";
            var roomLayoutConfigTemplate = 
                AssetDatabase.LoadAssetAtPath<RoomLayoutConfig>(ROOM_LAYOUT_CONFIG_PATH + ROOM_LAYOUT_TEMPLATE);

            if (roomLayoutConfigTemplate == null)
            {
                Debug.LogError("Room Layout Config template cannot be found!");
                return false;
            }

            var configAssetName = Regex.Replace(assetName, "([a-z])([A-Z])", "$1 $2");
            
            var newRoomLayoutConfig = Object.Instantiate((roomLayoutConfigTemplate));
            newRoomLayoutConfig.EditorSetLevelConfig(configAssetName, levelConfig);
            
            var defaultLayoutConfigs = 
                AssetDatabase.LoadAssetAtPath<RoomLayoutConfigs>(DEFAULT_ROOM_LAYOUT_CONFIGS_PATH);

            defaultLayoutConfigs.AddToConfig(newRoomLayoutConfig);
            
            EditorUtility.SetDirty(defaultLayoutConfigs);
            
            AssetDatabase.CreateAsset(newRoomLayoutConfig, newRoomLayoutConfigPath);
            AssetDatabase.SaveAssets();
            
            return true;
        }

        private static bool AddNewSceneToAddressableGroups(Scene scene, string assetName)
        {
            var settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(ADDRESSABLE_ASSET_SETTINGS);

            var groupName = $"Scene{assetName}";
            if (!settings)
            {
                Debug.LogError("Addressable settings cannot be found!");
                return false;
            }

            var schema = AssetDatabase.LoadAssetAtPath<AddressableAssetGroupSchema>(
                "Assets/AddressableAssetsData/AssetGroups/Schemas/SceneHangoutMVP_BundledAssetGroupSchema.asset");
            var group = settings.FindGroup(groupName);
            if (!group)
            {
                var schemas = new List<AddressableAssetGroupSchema> {schema};
                group = settings.CreateGroup(groupName, false, false, true, schemas,
                    typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            }

            var guid = AssetDatabase.AssetPathToGUID(SCENE_PATH + scene.name + ".unity");
            var e = settings.CreateOrMoveEntry(guid, group, false, false);
            e.SetLabel("default", true, false, false);
            e.SetAddress(scene.name, false);
            var entriesAdded = new List<AddressableAssetEntry> {e};
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);

            return true;
        }
    }
}
#endif
