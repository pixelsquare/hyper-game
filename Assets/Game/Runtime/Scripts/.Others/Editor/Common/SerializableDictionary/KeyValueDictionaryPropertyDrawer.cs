using Quantum;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [CustomPropertyDrawer(typeof(KeyValueDictionaryAttribute))]
    public class KeyValueDictionaryPropertyDrawer : PropertyDrawer
    {
        private bool isFolded;
        private ReorderableList reorderableList;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            isFolded = EditorGUI.BeginFoldoutHeaderGroup(pos, isFolded, label);

            if (isFolded)
            {
                var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;

                if (keyCollision)
                {
                    EditorGUILayout.HelpBox("Duplicate keys will not be serialized.", MessageType.Warning);
                }

                var list = property.FindPropertyRelative("list");

                if (reorderableList == null)
                {
                    reorderableList = new ReorderableList(property.serializedObject, list, true, true, true, true);
                    reorderableList.drawHeaderCallback = rect => DrawHeader(rect, label);
                    reorderableList.drawElementCallback = DrawElements;
                }

                property.serializedObject.Update();
                reorderableList.DoLayoutList();
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private void DrawHeader(Rect rect, GUIContent label)
        {
            EditorGUI.LabelField(rect, label);
        }

        private void DrawElements(Rect rect, int idx, bool isActive, bool isFocused)
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(idx);

            EditorGUI.LabelField(new Rect(rect.x, rect.y, 50f, EditorGUIUtility.singleLineHeight), "Key");

            rect = rect.AddXMin(50f);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 100f, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Key"),
                    GUIContent.none);

            rect = rect.AddXMin(110f);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Value"),
                    GUIContent.none);
        }
    }
}
