using UnityEngine;
using Quantum;
using Random = UnityEngine.Random;

namespace Kumu.Kulitan.Multiplayer
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Level Config")]
    public class LevelConfigScriptableObject : ScriptableObject
    {
        [SerializeField] private string sceneToLoad;
        [SerializeField] private string addressablesLabel;
        [SerializeField] private string previewIconSpriteAddressableAddress;
        [SerializeField] private RuntimeConfig config;
        [SerializeField] private AssetRefEntityPrototype playerPrototype;
        [SerializeField] private bool hasMinigame;
        private CustomPlayerInitialData customPlayerInitialData = new CustomPlayerInitialData();

        public string SceneToLoad => sceneToLoad;
        public string AddressablesLabel => addressablesLabel;
        public string PreviewIconSpriteAddressableAddress => previewIconSpriteAddressableAddress;
        public RuntimeConfig Config => config;
        public AssetRefEntityPrototype PlayerPrototype => playerPrototype;

        public CustomPlayerInitialData CustomPlayerInitialData => customPlayerInitialData;
        public bool HasMinigame => hasMinigame;

        public void RerollSeed()
        {
            config.Seed = Random.Range(0, int.MaxValue);
        }
        
        #if UNITY_EDITOR
        public void EditorSetLevelConfig(string sceneName, MapAsset mapAsset, bool hasMinigame = false)
        {
            sceneToLoad = sceneName;
            this.hasMinigame = hasMinigame;
            config.Map = mapAsset.Settings;
        }
        #endif
    }
}
