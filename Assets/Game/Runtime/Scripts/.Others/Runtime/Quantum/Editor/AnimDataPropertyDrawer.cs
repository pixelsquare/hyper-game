using System;
using System.Linq;
using Quantum.Prototypes;
using UnityEditor;
using UnityEngine;
using Kumu.Kulitan.Hangout;

[CustomPropertyDrawer(typeof(AnimDataGuid_Prototype))]
public class AnimDataPropertyDrawer : PropertyDrawer
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

        DrawAnimDataField(position, property.FindPropertyRelative("id"), label);
    }

    private void DrawAnimDataField(Rect position, SerializedProperty property, GUIContent label)
    {
        var animDataNames = animConfig.CharacterAnimations
            .Where(a => a is InteractableAnimationAsset)
            .Where(a => a.AssetObject.Guid.IsValid && !string.IsNullOrEmpty(a.DisplayName))
            .Select(a => new { Id = a.AssetObject.Guid.Value, DisplayName = $"{a.DisplayName}" })
            .OrderBy(a => a.DisplayName)
            .ToList();

        animDataNames.Insert(0, new { Id = 0L, DisplayName = "None" });

        var idx = Math.Max(0, animDataNames.FindIndex(a => a.Id.Equals(property.longValue)));
        idx = EditorGUI.Popup(position, "Animation To Play", idx, animDataNames.Select(a => a.DisplayName).ToArray());
        property.longValue = animDataNames[idx].Id;

        Resources.UnloadUnusedAssets();
    }
}
