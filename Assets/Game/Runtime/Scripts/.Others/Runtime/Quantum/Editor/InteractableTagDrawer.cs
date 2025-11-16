using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Hangout;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InteractableTag))]
public class InteractableTagDrawer : PropertyDrawer
{
    private AnimationConfig animConfig;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (animConfig == null)
        {
            var animConfigs = AssetDatabase.FindAssets($"t:{nameof(AnimationConfig)}")
                                           .Select(a => AssetDatabase.GUIDToAssetPath(a))
                                           .Select(a => AssetDatabase.LoadAssetAtPath<AnimationConfig>(a))
                                           .ToArray();

            var animConfigLen = animConfigs.Length;

            if (animConfigLen != 1)
            {
                EditorGUI.LabelField(position, animConfigLen == 0
                                ? "Error: Missing AnimationConfig"
                                : "Error: Too many AnimationConfig",
                        EditorStyles.boldLabel);
                return;
            }

            animConfig = animConfigs[0];
        }

        DrawInteractableTagField(position, property.FindPropertyRelative("tag"), label);
    }

    private void DrawInteractableTagField(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();

        var interactableTags = new List<string>(animConfig.InteractableIconTags.Keys);
        var idx = Math.Max(0, interactableTags.FindIndex(a => a.Equals(property.stringValue)));
        idx = EditorGUI.Popup(position, label.text, idx, interactableTags.ToArray());
        property.stringValue = interactableTags[idx];

        if (EditorGUI.EndChangeCheck())
        {
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
