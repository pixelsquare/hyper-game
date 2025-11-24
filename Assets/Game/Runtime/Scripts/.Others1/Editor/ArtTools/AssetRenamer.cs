using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Santelmo.Rinsurv.Editor
{
    public class AssetRenamer : OdinEditorWindow
    {
        #region Enumerations
        public enum SourceType { assign, selection, folder }
        public enum AssetType { environment,character, vfx, generic, custom }
        public enum GenericType { model, texture, animation, material }
        public enum EnvironmentType { foreground, ground, groundDetail, obstacle, atlas }
        public enum CharacterAssetType { animation, sprite, spriteSheet }
        public enum SpriteDirection {_0, _45, _90, _135, _180, _225, _270, _315 }
        public enum VFXAssetType { animation, material, sprite }
        public enum SuffixType { constant, consecutive}
        public enum LegacyType {basic, ibabawnon, idalmunon, kahangian, lupanon, tubignon }
        #endregion

        [MenuItem("Santelmo/Art Tools/Asset Renamer")]
        private static void OpenWindow()
        {
            GetWindow<AssetRenamer>().Show();
            var window = GetWindow<AssetRenamer>();
            Vector2 windowSize = new Vector2(500, 500);
            window.minSize = windowSize;
            window.maxSize = windowSize;
        }

        #region Private Properties
        private string environmentTypeString;
        private string genericPrefixString;
        private string vfxPrefixString;
        private string spriteDirectionString;
        private string characterPrefixTypeString;
        private string legacyTypeString;
        private string sampleName;
        private bool canRename;
        #endregion

        [BoxGroup("Options", centerLabel: true)]
        [HorizontalGroup("Options/split"),VerticalGroup("Options/split/right"), DisplayAsString(Alignment = TextAlignment.Center), HideLabel]
        public string sourceTypeTitle = "Source Type";

        [VerticalGroup("Options/split/right"), EnumToggleButtons, HideLabel, GUIColor("white")]
        public SourceType sourceType;

        [ShowIf("@ this.sourceType == SourceType.assign"),VerticalGroup("Options/split/right"), GUIColor("white") , LabelWidth (40), Required]
        public Object asset;

        [ShowIf("@ this.sourceType == SourceType.folder"), VerticalGroup("Options/split/right"), GUIColor("white"), LabelWidth(80), Required]
        public DefaultAsset assetsFolder;

        [ShowIf("@ this.sourceType == SourceType.selection"), VerticalGroup("Options/split/right"), GUIColor("white"), LabelWidth(100), 
            DisplayAsString(Alignment = TextAlignment.Left)]
        public int selectedAssets;

        [VerticalGroup("Options/split/left"), DisplayAsString(Alignment = TextAlignment.Center), HideLabel]
        public string assetTypeTitle = "Asset Type";

        [VerticalGroup("Options/split/left"), EnumToggleButtons, HideLabel, GUIColor("white"), ]
        public AssetType assetType;

        [Space(5), Title("Environment Type", "", TitleAlignments.Centered), ShowIf("assetType", AssetType.environment), EnumToggleButtons, HideLabel, GUIColor("cyan")]
        public EnvironmentType environmentType;

        [Space(5), Title("Generic Asset Type", "", TitleAlignments.Centered), ShowIf("assetType", AssetType.generic), EnumToggleButtons, HideLabel, GUIColor("cyan")]
        public GenericType genericType;

        [Space(5), Title("Character Asset Type", "", TitleAlignments.Centered), ShowIf("assetType", AssetType.character), EnumToggleButtons, HideLabel, GUIColor("cyan")]
        public CharacterAssetType characterAssetType;

        [Space(5), Title("Sprite Direction", "", TitleAlignments.Centered), ShowIf("@ this.assetType == AssetType.character && this.characterAssetType != CharacterAssetType.spriteSheet"), 
            EnumToggleButtons, HideLabel, GUIColor("cyan")]
        public SpriteDirection spriteDirection;

        [Space(5), Title("VFX Asset Type", "", TitleAlignments.Centered), ShowIf("assetType", AssetType.vfx), EnumToggleButtons, HideLabel, GUIColor("cyan")]
        public VFXAssetType vFXAssetType;

        [Title("Legacy Type", "", TitleAlignments.Centered), ShowIf("assetType", AssetType.vfx), EnumToggleButtons, HideLabel, GUIColor("cyan")]
        public LegacyType legacyType;

        [LabelWidth(125), ShowIf("@ this.assetType != AssetType.custom")]
        public bool keepAssetName;

        [LabelWidth (125), ShowIf("@ this.assetType != AssetType.custom")]
        public bool keepSuffix;

        [Indent, ShowIf("@ this.keepSuffix == false && this.assetType != AssetType.custom"), EnumToggleButtons, LabelWidth(125)]
        public SuffixType suffixType;

        [Indent, ShowIf("@ this.keepSuffix == false && this.suffixType == SuffixType.constant && this.assetType != AssetType.custom"), LabelWidth(125)]
        public int constantSuffix;

        [ShowIf("@ this.assetType == AssetType.environment && this.environmentType != EnvironmentType.atlas"), LabelWidth(125)]
        public bool keepBiomeName;

        [ShowIf("@ this.assetType == AssetType.vfx"), LabelWidth(125)]
        public bool keepHeroName;

        [ShowIf("@ this.assetType == AssetType.character && this.characterAssetType != CharacterAssetType.spriteSheet"), LabelWidth(125)]
        public bool keepAnimationName;

        [ShowIf("@ this.assetType == AssetType.environment && this.environmentType != EnvironmentType.atlas && this.keepBiomeName == false"), 
            BoxGroup("Inputs", centerLabel: true), HorizontalGroup("Inputs/left"), LabelWidth(80)]
        public string biomeName;

        [ShowIf("@ this.assetType == AssetType.vfx && this.keepHeroName == false"),
           BoxGroup("Inputs", centerLabel: true), HorizontalGroup("Inputs/left"), LabelWidth(80)]
        public string heroName;

        [ShowIf("@ this.keepAssetName == false"),HorizontalGroup("Inputs/left"), LabelWidth(80)]
        public string assetName;

        [ShowIf("@ this.assetType == AssetType.character && this.characterAssetType != CharacterAssetType.spriteSheet && this.keepAnimationName == false"),
            BoxGroup("Inputs", centerLabel: true), HorizontalGroup("Inputs/left"), LabelWidth(100)]
        public string animationName;

        [Title("@ sampleName", "Sample Name", TitleAlignments.Centered, HorizontalLine = false),
            DisplayAsString(), HideLabel]
        public string namePreview = "";

        [Button(ButtonSizes.Large), GUIColor("green"), EnableIf("@ this.canRename == true")]
        public void RenameAsset()
        {
            switch (sourceType)
            {
                case SourceType.assign:
                    Rename(asset, 1);
                    break;
                case SourceType.selection:

                    List<Object> selectedObjects = Selection.objects.ToList();

                    foreach (Object obj in selectedObjects)
                    {
                        Rename(obj, selectedObjects.IndexOf(obj) + 1);
                    }

                    break;
                case SourceType.folder:

                    List<Object> objects = AssetDatabase.FindAssets("t:Object", new[] { AssetDatabase.GetAssetPath(assetsFolder) })
                       .Select(AssetDatabase.GUIDToAssetPath)
                       .Select(AssetDatabase.LoadAssetAtPath<Object>).ToList();

                    foreach (Object obj in objects)
                    {
                        Rename(obj, objects.IndexOf(obj) + 1);
                    }

                    break;
            } 
        }

        private void Rename(Object asset, int suffixInt)
        {
            string combinedName = "";
            string[] nameSplit = asset.name.Split(char.Parse("_"));
            string suffix = "";
            string newAssetName = "";

            if (!keepAssetName)
                newAssetName = assetName;

            if (!keepSuffix)
            {
                if (suffixType == SuffixType.constant)
                {
                    if (assetType == AssetType.character)
                    {
                        if (characterAssetType == CharacterAssetType.sprite)
                            suffix = constantSuffix.ToString("0000");
                        else
                            suffix = constantSuffix.ToString("00");
                    }
                    else
                        suffix = constantSuffix.ToString("00");
                }
                else
                {
                    if (assetType == AssetType.character)
                    {
                        if (characterAssetType == CharacterAssetType.sprite)
                            suffix = suffixInt.ToString("0000");
                        else
                            suffix = suffixInt.ToString("00");
                    }
                    else
                        suffix = suffixInt.ToString("00");
                }
            }
            else
                suffix = nameSplit[nameSplit.Count() - 1];

            switch (assetType)
            {
                case AssetType.environment:

                    string newBiomeName = "";

                    if (!keepBiomeName)
                        newBiomeName = biomeName;
                    else
                        newBiomeName = nameSplit[0];

                    if (environmentType != EnvironmentType.atlas)
                    {
                        if (keepAssetName)
                            newAssetName = nameSplit[2];
               
                        combinedName = $"{newBiomeName}_{environmentTypeString}_{newAssetName}_{suffix}";
                    }
                    else
                    {
                        if (keepAssetName)
                            newAssetName = nameSplit[0];
                        combinedName = $"{newAssetName}_{environmentTypeString}_{suffix}";
                    }

                    break;
                case AssetType.character:

                    string newAnimationName = "";

                    if (keepAnimationName)
                        newAnimationName = nameSplit[1];
                    else
                        newAnimationName = animationName;

                    if (keepAssetName)
                        newAssetName = nameSplit[0];

                    switch (characterAssetType)
                    {
                        case CharacterAssetType.animation:

                            combinedName = $"{newAssetName}_{newAnimationName}_{spriteDirectionString}_{suffix}";

                            break;
                        case CharacterAssetType.sprite:

                            combinedName = $"{newAssetName}_{newAnimationName}{spriteDirection}_{suffix}";

                            break;
                        case CharacterAssetType.spriteSheet:

                            combinedName = $"{newAssetName}_animations_{suffix}";

                            break;
                    }

                    break;
                case AssetType.vfx:

                    string newHeroName = "";

                    if (keepHeroName)
                        newHeroName = nameSplit[1];
                    else
                        newHeroName = heroName;

                    if (keepAssetName)
                        newAssetName = nameSplit[3];

                    combinedName = $"{vfxPrefixString}_{newHeroName}_{legacyTypeString}_{newAssetName}_{suffix}";

                    break;
                case AssetType.generic:

                    if (keepAssetName)
                        newAssetName = nameSplit[1];

                    combinedName = $"{genericPrefixString}_{newAssetName}_{suffix}";

                    break;
                case AssetType.custom:

                    combinedName = assetName;

                    break;
            }

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(asset), combinedName);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void Update()
        {
            selectedAssets = Selection.count;

            switch (sourceType)
            {
                case SourceType.assign:
                    if (asset == null)
                        canRename = false;
                    else
                        canRename = true;
                    break;
                case SourceType.selection:
                    if (Selection.objects.ToList().Count == 0)
                        canRename = false;
                    else
                        canRename = true;
                    break;
                case SourceType.folder:
                    if (assetsFolder == null)
                        canRename = false;
                    else
                        canRename = true;
                    break;
            }

            switch (assetType)
            {
                case AssetType.environment:
                    switch (environmentType)
                    {
                        case EnvironmentType.foreground:
                            environmentTypeString = "fg";
                            break;
                        case EnvironmentType.ground:
                            environmentTypeString = "gr";
                            break;
                        case EnvironmentType.groundDetail:
                            environmentTypeString = "grd";
                            break;
                        case EnvironmentType.obstacle:
                            environmentTypeString = "obs";
                            break;
                        case EnvironmentType.atlas:
                            environmentTypeString = "atlas";
                            break;
                    }

                    if (environmentType != EnvironmentType.atlas)
                        sampleName = $"biomeName_{environmentTypeString}_assetName_01";
                    else
                        sampleName = $"assetName_{environmentTypeString}_01";

                    break;
                case AssetType.character:

                    switch (spriteDirection)
                    {
                        case SpriteDirection._0:
                            spriteDirectionString = "forward";
                            break;
                        case SpriteDirection._45:
                            spriteDirectionString = "forwardLeft";
                            break;
                        case SpriteDirection._90:
                            spriteDirectionString = "Left";
                            break;
                        case SpriteDirection._135:
                            spriteDirectionString = "backwardLeft";
                            break;
                        case SpriteDirection._180:
                            spriteDirectionString = "backward";    
                            break;
                        case SpriteDirection._225:
                            spriteDirectionString = "backwardRight";
                            break;
                        case SpriteDirection._270:
                            spriteDirectionString = "right";
                            break;
                        case SpriteDirection._315:
                            spriteDirectionString = "forwardRight";
                            break;
                    }

                    switch (characterAssetType)
                    {
                        case CharacterAssetType.animation:
                            sampleName = $"assetName_animationName_{spriteDirectionString}_01";
                            break;
                        case CharacterAssetType.sprite:
                            sampleName = $"assetName_animationName{spriteDirection}_0000";
                            break;
                        case CharacterAssetType.spriteSheet:
                            sampleName = $"assetName_animations_01";
                            break;
                    }

                    break;
                case AssetType.vfx:

                    switch (vFXAssetType)
                    {
                        case VFXAssetType.animation:
                            vfxPrefixString = "anim";
                            break;
                        case VFXAssetType.material:
                            vfxPrefixString = "mat";
                            break;
                        case VFXAssetType.sprite:
                            vfxPrefixString = "spt";
                            break;
                    }

                    legacyTypeString = legacyType.ToString();

                    sampleName = $"{vfxPrefixString}_heroName_{legacyTypeString}_assetName_01";

                    break;
                case AssetType.generic:

                    switch (genericType)
                    {
                        case GenericType.model:
                            genericPrefixString = "mod";
                            break;
                        case GenericType.texture:
                            genericPrefixString = "tex";
                            break;
                        case GenericType.animation:
                            genericPrefixString = "anim";
                            break;
                        case GenericType.material:
                            genericPrefixString = "mat";
                            break;
                    }

                    sampleName = $"{genericPrefixString}_assetName_01";

                    break;
                case AssetType.custom:
                    keepAssetName = false;
                    sampleName = assetName;
                    break;
            }
        }
    }
}
