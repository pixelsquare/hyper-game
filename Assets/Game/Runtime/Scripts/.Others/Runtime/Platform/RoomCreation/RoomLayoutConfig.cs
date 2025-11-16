using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Room Layout Config", fileName = "Room Layout 1")]
    public class RoomLayoutConfig : ScriptableObject
    {
        [SerializeField] private string layoutName;
        [SerializeField] private Sprite icon;
        [SerializeField] private LevelConfigScriptableObject levelConfig;
        [SerializeField] private bool isTestRoom = false;
        
        public string LayoutName => layoutName;
        public Sprite Icon => icon;
        public LevelConfigScriptableObject LevelConfig => levelConfig;
        public string SceneToLoad => LevelConfig.SceneToLoad;
        public bool IsTestRoom => isTestRoom;
        
#if UNITY_EDITOR
        public void EditorSetLevelConfig(string layoutName, LevelConfigScriptableObject levelConfig)
        {
            this.layoutName = layoutName;
            this.levelConfig = levelConfig;
        }
#endif
    }
}
