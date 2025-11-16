using UnityEditor;

namespace Kumu.Kulitan.Avatar
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ItemCategoryConfig))]
    public class ItemCategoryConfigEditor : Editor
    {
        SerializedProperty itemTagField;
        SerializedProperty itemTypeField;
        SerializedProperty deselectedTypesField;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawDefaultInspector();

            itemTagField.stringValue = EditorGUILayout.TextField("Item Category", serializedObject.targetObject.name);
            itemTypeField.intValue = (int) (AvatarItemType) EditorGUILayout.EnumPopup("Item Type", (AvatarItemType) itemTypeField.intValue);
            deselectedTypesField.intValue = (int) (AvatarItemType) EditorGUILayout.EnumFlagsField("Deselected Types", (AvatarItemType) deselectedTypesField.intValue);

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            itemTagField = serializedObject.FindProperty("itemCategory");
            itemTypeField = serializedObject.FindProperty("itemType");
            deselectedTypesField = serializedObject.FindProperty("deselectedTypes");
        }
    }
}
