using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        private static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
        private static readonly float vertSpace = EditorGUIUtility.standardVerticalSpacing;

        public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
        {
            var list = property.FindPropertyRelative("list");
            var fieldName = ObjectNames.NicifyVariableName(fieldInfo.Name);
            var currentPos = new Rect(lineHeight, pos.y, pos.width, lineHeight);
            EditorGUI.PropertyField(currentPos, list, new GUIContent(fieldName), true);

            var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
            if (keyCollision)
            {
                currentPos.y += EditorGUI.GetPropertyHeight(list, true) + vertSpace;
                var entryPos = new Rect(lineHeight, currentPos.y, pos.width, lineHeight * 2f);
                EditorGUI.HelpBox(entryPos, "Duplicate keys will not be serialized.", MessageType.Warning);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalHeight = 0f;

            // Height of KeyValue list.
            var listProp = property.FindPropertyRelative("list");
            totalHeight += EditorGUI.GetPropertyHeight(listProp, true);

            // Height of key collision warning.
            var keyCollision = property.FindPropertyRelative("keyCollision").boolValue;
            if (keyCollision)
            {
                totalHeight += lineHeight * 2f + vertSpace;
            }

            return totalHeight;
        }
    }
}
