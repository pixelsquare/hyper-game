using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace Santelmo.Rinsurv.Editor
{
    public class DirectoryGenerator : OdinEditorWindow
    {
        private string enemyDirectoryTemplatePath = "Assets/_Source/Content/ScriptableObjects/ArtTools/EnemyDirectoryTemplate.asset";
        private string heroDirectoryTemplatePath = "Assets/_Source/Content/ScriptableObjects/ArtTools/HeroDirectoryTemplate.asset";
        private string biomeDirectoryTemplatePath = "Assets/_Source/Content/ScriptableObjects/ArtTools/BiomeDirectoryTemplate.asset";
        private string animatorTemplatePath = "Assets/_Source/Content/Visuals/Characters/ctrl_AnimatorTemplate_01.controller";
        private string characterMaterialTemplatePath = "Assets/_Source/Content/Visuals/Characters/mat_characterMaterialTemplate_01.mat";
        private string characterHitMaterialTemplatePath = "Assets/_Source/Content/Visuals/Characters/mat_characterHitMaterialTemplate_01.mat";
        private DirectoryScriptableObject directoryScriptableObject;
        private bool canGenerateDirectory;
        #region Window
        private static Vector2 windowSize = new Vector2(300, 220);

        [MenuItem("Santelmo/Art Tools/Directory Generator")]
        private static void ShowWindow()
        {
            EditorWindow window = GetWindow<DirectoryGenerator>("Directory Generator");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, windowSize.x, windowSize.y);
            window.minSize = new Vector2(windowSize.x, windowSize.y);
            window.maxSize = new Vector2(windowSize.x, windowSize.y);
        }
        #endregion
        public enum DirectoryType {enemy, hero, biome }

        [BoxGroup("Directory Type", centerLabel: true), EnumToggleButtons, HideLabel, GUIColor("white")]
        public DirectoryType directoryType;

        [BoxGroup("Output", centerLabel: true), GUIColor("white"), Required, LabelWidth(100)]
        public string directoryName = "";

        [BoxGroup("Output", centerLabel: true), GUIColor("white"), Required, LabelWidth (100)]
        public DefaultAsset outputFolder;

        [Button(ButtonSizes.Large), GUIColor("green"), EnableIf("@ this.canGenerateDirectory == true")]
        public void GenerateDirectory()
        {
            switch (directoryType)
            {
                case DirectoryType.enemy:
                    directoryScriptableObject = AssetDatabase.LoadAssetAtPath<DirectoryScriptableObject>(enemyDirectoryTemplatePath);
                    break;
                case DirectoryType.hero:
                    directoryScriptableObject = AssetDatabase.LoadAssetAtPath<DirectoryScriptableObject>(heroDirectoryTemplatePath);
                    break;
                case DirectoryType.biome:
                    directoryScriptableObject = AssetDatabase.LoadAssetAtPath<DirectoryScriptableObject>(biomeDirectoryTemplatePath);
                    break;
            }

            directoryScriptableObject.directory.directoryName = directoryName;
            directoryScriptableObject.directory.CreateDirectory(AssetDatabase.GetAssetPath(outputFolder));

            if (directoryType != DirectoryType.biome)
            {
                AssetDatabase.CopyAsset(animatorTemplatePath, $"{AssetDatabase.GetAssetPath(outputFolder)}/{directoryName}/Animations/3D/ctrl_{directoryName}_01.controller");
                AssetDatabase.CopyAsset(characterMaterialTemplatePath, $"{AssetDatabase.GetAssetPath(outputFolder)}/{directoryName}/Materials/mat_{directoryName}_01.mat");
                AssetDatabase.CopyAsset(characterHitMaterialTemplatePath, $"{AssetDatabase.GetAssetPath(outputFolder)}/{directoryName}/Materials/mat_{directoryName}Hit_01.mat");
                AssetDatabase.Refresh();
            }
        }

        private void Update()
        {
            if (directoryName != string.Empty && outputFolder != null)
                canGenerateDirectory = true;
            else
                canGenerateDirectory = false;
        }
    }

}
