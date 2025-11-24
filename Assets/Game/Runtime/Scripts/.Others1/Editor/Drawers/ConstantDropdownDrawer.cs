using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    [CustomPropertyDrawer(typeof(ConstantDropdownAttribute))]
    public class ConstantDropdownDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var attr = attribute as ConstantDropdownAttribute;
            GetConstants(attr?.Type, out var fields);
            var index = Math.Max(0, Array.FindIndex(fields.ToArray(), x => x.Item2.Equals(property.stringValue, StringComparison.OrdinalIgnoreCase)));

            EditorGUI.BeginChangeCheck();
            index = EditorGUI.Popup(position, index, fields.Select(x => x.Item1).ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = fields[index].Item2;
            }

            EditorGUI.EndProperty();
        }

        private void GetConstants(Type type, out List<(string, string)> fields)
        {
            fields = new List<(string, string)>();
            fields.Add(new ValueTuple<string, string>("< none >", string.Empty));

            var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var fieldInfo in fieldInfos)
            {
                if (!fieldInfo.IsLiteral || fieldInfo.IsInitOnly)
                {
                    continue;
                }

                fields.Add(new ValueTuple<string, string>(fieldInfo.Name, fieldInfo.GetValue(null).ToString()));
            }
        }
    }
}
