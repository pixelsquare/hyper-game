#if UNITY_EDITOR
using System.Linq;
using Kumu.Kulitan.Multiplayer;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor.SceneTemplate;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.EditorTools
{
    public class HangoutScenePipeline : ISceneTemplatePipeline
    {
        private const string LEVELDATA_PATH = "Assets/Resources/Quantum/Hangout/Levels";
        private const string LEVELCONFIG_PATH = "Assets/Configs/Portal/HangoutLevels/";
        private const string LEVELCONFIG_TEMPLATE = "HangoutLevelConfig.asset";
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
            }
            Debug.Log($"Setup successful {scene.name}");
        }

        private static bool SetupSceneForQuantum(Scene scene)
        {
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
            var mapAsset = ScriptableObject.CreateInstance<MapAsset>();
            var assetPath = AssetDatabase.CreateFolder(LEVELDATA_PATH, scene.name);
            if (assetPath == string.Empty)
            {
                Debug.LogError("Folder creation failed.");
                return false;
            }
            
            AssetDatabase.CreateAsset(mapAsset, AssetDatabase.GUIDToAssetPath(assetPath) + "/" + scene.name + SCRIPTABLE_EXTENSION);
            AssetDatabase.SaveAssets();

            mapData.Asset = mapAsset;
            MapDataBaker.BakeMapData(mapData, true);

            EditorSceneManager.SaveScene(scene);
            
            // Create a LevelConfig from a template file
            var newPath = LEVELCONFIG_PATH + scene.name + SCRIPTABLE_EXTENSION;
            var template =
                AssetDatabase.LoadAssetAtPath<LevelConfigScriptableObject>(LEVELCONFIG_PATH + LEVELCONFIG_TEMPLATE);
            if (template == null)
            {
                Debug.LogError("LevelConfig template cannot be found!");
                return false;
            }

            var newLevelConfig = Object.Instantiate(template);
            newLevelConfig.EditorSetLevelConfig(scene.name, mapAsset);
            AssetDatabase.CreateAsset(newLevelConfig, newPath);
            AssetDatabase.SaveAssets();

            return true;
        }
    }
}
#endif
