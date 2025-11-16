using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Kumu.Kulitan.Avatar
{
    public static class ItemDatabaseExport
    {
        
        [MenuItem("Tools/Avatar Customization/Database/Export Items")]
        public static void ExportDatabaseCsv()
        {
            var path = EditorUtility.SaveFilePanel(
                "Export Item Database",
                Application.dataPath,
                "item-database.csv",
                ".csv");

            var database = AssetDatabase.LoadAssetAtPath<ItemDatabase>(AvatarEditorConstants.PATH_ITEM_DATABASE);
            var configs = from config in database.ItemConfigs 
                          where !config.Unpurchaseable
                          select config;
            
            var lineCt = 0;

            using (var writer = new StreamWriter(path))
            {
                foreach (var config in configs)
                {
                    var line = $"{config.Data.itemId}";
                    writer.WriteLine(line);
                    ++lineCt;
                }
            }

            EditorUtility.DisplayDialog(
                "Export Finished",
                $"Written {lineCt} lines to {path}",
                "Confirm"
            );
        }
    }
}
