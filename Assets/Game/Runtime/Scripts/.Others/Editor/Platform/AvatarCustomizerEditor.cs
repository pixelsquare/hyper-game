using UnityEngine;
using UnityEditor;

namespace Kumu.Kulitan.Avatar
{
    [CustomEditor(typeof(AvatarCustomizer))]
    public class AvatarCustomizerEditor : Editor
    {
        private bool isTesting;
        private Color color = Color.white;
        private AvatarItemConfig itemConfig;

        private AvatarCustomizer avatarCustomizer => (AvatarCustomizer)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            isTesting = GUILayout.Toggle(isTesting, "Test Mode");

            if (!isTesting)
            {
                return;
            }

            itemConfig = (AvatarItemConfig)EditorGUILayout.ObjectField(itemConfig, typeof(AvatarItemConfig), true);
            color = EditorGUILayout.ColorField(color);

            if (GUILayout.Button("Equip"))
            {
                itemConfig.SetStateColor(color);
                avatarCustomizer.SelectItem(itemConfig);
            }
        }
    }
}
