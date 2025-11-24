using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Linq;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

namespace Santelmo.Rinsurv.Editor
{
    public class TileGenerator : OdinEditorWindow
    {
        private static Vector2 windowSize = new Vector2(300, 320);

        private List<Object> selectedTiles = new List<Object>();
        private List<Tile> tilesInFolder = new List<Tile>();

        private bool canFixTiles;
        private bool canGenerateTiles;

        [MenuItem("Santelmo/Art Tools/Tile Generator")]
        private static void ShowWindow()
        {
            EditorWindow spriteGeneratorWindow = GetWindow<TileGenerator>("Tile Generator");

            spriteGeneratorWindow.position = new Rect(Screen.width / 2, Screen.height / 2, windowSize.x, windowSize.y);
            spriteGeneratorWindow.minSize = new Vector2(windowSize.x, windowSize.y);
            spriteGeneratorWindow.maxSize = new Vector2(windowSize.x, windowSize.y);
        }
        public enum ProcessType {tileGeneration, tileFixer }
        public enum TileSourceType { single, selection, folder }

        [BoxGroup("Options", centerLabel: true),EnumToggleButtons, HideLabel, GUIColor("white")]
        public ProcessType processType;

        [BoxGroup("Sprite Sheet", centerLabel: true), PreviewField(70, Alignment = ObjectFieldAlignment.Center), HideLabel, Required]
        public Sprite spriteSheet;

        [BoxGroup("Tile Source Type", centerLabel: true),EnumToggleButtons, HideLabel, GUIColor("cyan"), 
            ShowIf("@ this.processType == ProcessType.tileFixer")]
        public TileSourceType tileSourceType;
        [BoxGroup("Tile Source Type"), ShowIf("@ this.tileSourceType == TileSourceType.folder && this.processType == ProcessType.tileFixer"), Required]
        public DefaultAsset sourceFolder;
        [BoxGroup("Tile Source Type"), ShowIf("@ this.tileSourceType == TileSourceType.single && this.processType == ProcessType.tileFixer"), Required]
        public Tile tile;
        [BoxGroup("Tile Source Type"), ShowIf("@ this.tileSourceType == TileSourceType.selection && this.processType == ProcessType.tileFixer"), 
            DisplayAsString(Alignment = TextAlignment.Left)]
        public int selectedAssets;

        [BoxGroup("Output Folder", centerLabel: true), ShowIf("@ this.processType == ProcessType.tileGeneration"), Required, HideLabel]
        public DefaultAsset outputFolder;

        [Button(ButtonSizes.Large), GUIColor("green"), ShowIf("@ this.processType == ProcessType.tileFixer"), EnableIf("@ this.canFixTiles == true")]
        public void FixTiles()
        {
            switch (tileSourceType)
            {
                case TileSourceType.single:
                    InititateFixTiles(tile);
                    break;
                case TileSourceType.selection:
                    foreach (Object selected in selectedTiles)
                    {
                        InititateFixTiles(selected);
                    }
                    break;
                case TileSourceType.folder:
                    foreach (Object asset in tilesInFolder)
                    {
                        InititateFixTiles(asset);
                    }
                    break;
            }
        }

        [Button(ButtonSizes.Large), GUIColor("green"), ShowIf("@ this.processType == ProcessType.tileGeneration"), EnableIf("@ this.canGenerateTiles == true")]
        public void GenerateTiles()
        {
            InitiateGenerateTiles(spriteSheet);
        }

        #region Unity Functions
        private void OnSelectionChange()
        {
            selectedTiles.Clear();
            this.Repaint();
        }
        private void Update()
        {
            switch (processType)
            {
                case ProcessType.tileGeneration:
                    if (outputFolder != null && spriteSheet != null)
                        canGenerateTiles = true;
                    else
                        canGenerateTiles = false;
                    break;
                case ProcessType.tileFixer:
                    selectedTiles = Selection.GetFiltered(typeof(Tile), SelectionMode.Assets).ToList();
                    selectedAssets = selectedTiles.Count();
                    if (spriteSheet != null)
                    {
                        switch (tileSourceType)
                        {
                            case TileSourceType.single:
                                if (tile != null)
                                    canFixTiles = true;
                                else
                                    canFixTiles = false;
                                break;
                            case TileSourceType.selection:
                                if (selectedTiles.Count != 0)
                                    canFixTiles = true;
                                else
                                    canFixTiles = false;
                                break;
                            case TileSourceType.folder:
                                GetTiles(AssetDatabase.GetAssetPath(sourceFolder));
                                if (tilesInFolder.Count != 0)
                                    canFixTiles = true;
                                else
                                    canFixTiles = false;
                                break;
                        }
                    }
                    break;
            }
            
        }
        #endregion
        #region Editor Functions
       
        private void GetTiles(string path)
        {
            List<Texture> tiles = new List<Texture>();

            tilesInFolder = AssetDatabase.FindAssets("t:Tile", new[] { path })
               .Select(AssetDatabase.GUIDToAssetPath)
               .Select(AssetDatabase.LoadAssetAtPath<Tile>).ToList();
        }
        private void InititateFixTiles(Object asset)
        {
            List<Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(spriteSheet)).OfType<Sprite>().ToList();
            Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(AssetDatabase.GetAssetPath(asset));

            string tileName = tile.name.ToLower();

            foreach (Sprite sprite in sprites)
            {
                string spriteName = sprite.name.ToLower();

                if (tileName == spriteName)
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(tile), sprite.name);
                    tile.sprite = sprite;

                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void InitiateGenerateTiles(Object asset)
        {
            List<Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(asset)).OfType<Sprite>().ToList();

            foreach (Sprite sprite in sprites)
            {
                SortTiles(sprite, "_fg_", "Foreground");
                SortTiles(sprite, "_gr_", "Ground");
                SortTiles(sprite, "_grd_", "GroundDetails");
                SortTiles(sprite, "_obs_", "Obstacles");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void SortTiles(Sprite sprite, string type, string folder)
        {
            Tile tile = CreateInstance<Tile>();
            tile.sprite = sprite;

            if (sprite.name.Contains(type))
            {
                if (!AssetDatabase.IsValidFolder($"{AssetDatabase.GetAssetPath(outputFolder)}/{folder}"))
                    AssetDatabase.CreateFolder(AssetDatabase.GetAssetPath(outputFolder), folder);

                AssetDatabase.CreateAsset(tile, $"{AssetDatabase.GetAssetPath(outputFolder)}/{folder}/{sprite.name}.asset");
            }
        }
        #endregion
    }
}
