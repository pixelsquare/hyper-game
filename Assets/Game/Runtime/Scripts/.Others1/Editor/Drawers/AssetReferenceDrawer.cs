using System.IO;
using System.Linq;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    [CustomPropertyDrawer(typeof(AssetReferenceAttribute))]
    public class AssetReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var attr = attribute as AssetReferenceAttribute;

            Object obj = null;
            AddressableAssetEntry entry;

            if (!string.IsNullOrEmpty(property.stringValue))
            {
                var assetName = Path.GetFileNameWithoutExtension(property.stringValue);
                var guids = AssetDatabase.FindAssets(assetName);

                var assetGuid = guids.First();
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                obj = AssetDatabase.LoadAssetAtPath(assetPath, attr?.Type);

                if (!IsAddressableAsset(assetGuid, out entry))
                {
                    obj = null;
                    property.stringValue = string.Empty;
                }
            }

            EditorGUI.BeginChangeCheck();
            obj = SirenixEditorFields.UnityObjectField(position, obj, attr?.Type, false);

            if (EditorGUI.EndChangeCheck())
            {
                if (IsAddressableAsset(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)), out entry))
                {
                    property.stringValue = entry?.address;
                }
                else
                {
                    Debug.LogError($"Asset is not an addressable! [{obj.name}]");
                    property.stringValue = string.Empty;
                }
            }

            EditorGUI.EndProperty();
        }

        private bool IsAddressableAsset(string assetGuid, out AddressableAssetEntry entry)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            entry = settings.FindAssetEntry(assetGuid, true);
            return entry != null;
        }
    }
}
