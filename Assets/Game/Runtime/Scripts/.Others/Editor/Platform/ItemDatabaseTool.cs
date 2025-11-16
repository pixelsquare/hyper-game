using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Kumu.Extensions;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;
using Path = System.IO.Path;

namespace Kumu.Kulitan.Avatar
{
    public static class ItemDatabaseTool
    {
        [MenuItem("Tools/Avatar Customization/Database/Generate Items")]
        private static void GenerateItems()
        {
            PromptClearDatabase();
            PromptCreateConfigs();
            PromptPopulateAddressables();
            PromptClearOldConfigs();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void PromptCreateConfigs()
        {
            var title = "Generate Avatar Configs";
            var message = $"Creating AvatarItemConfigs under the directory: {AvatarEditorConstants.DIRECTORY_ITEM_CONFIGS}";
            var confirm = "Confirm";
            var defaultDirectory = AvatarEditorConstants.DIRECTORY_ITEM_CONFIGS;

            EditorUtility.DisplayDialog(title, message, confirm);

            CreateConfigs(defaultDirectory);
        }

        private static void PromptClearDatabase()
        {
            var message = "Avatar item database will be overwritten with generated data.";
            var title = "Clear Item Database";
            var confirm = "Confirm";

            if (!EditorUtility.DisplayDialog(title, message, confirm))
            {
                return;
            }

            var database = LoadItemDatabase();
            database.ItemConfigs.Clear();
            EditorUtility.SetDirty(database);
        }

        private static void CreateConfigs(string directory)
        {
            var database = LoadItemDatabase();

            var idx = 0;
            var texGuids = GetTextureGuids();
            foreach (var guid in texGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Texture>(path);

                var filename = asset.name;
                var info = asset.name.Split("_");
                var configWrapper = ItemFilenameParser.ToItemConfig(info[0], filename);

                idx++;
                EditorUtility.DisplayProgressBar("Creating Configurations",
                    $"Creating configs for {filename} ... ({idx} / {texGuids.Length})",
                    (float)idx / texGuids.Length);

                if (configWrapper == null)
                {
                    continue;
                }

                var config = configWrapper.CreateConfig(guid, filename);
                var configPath = $"{directory}/{config.Data.itemName}.asset";
                CreateAsset(config, configPath);
                EditorUtility.SetDirty(config);

                database.ItemConfigs.Add(config);
            }

            EditorUtility.SetDirty(database);
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Handles retaining values that should not be overwritten.
        /// </summary>
        private static void CreateAsset(AvatarItemConfig config, string path)
        {
            var oldConfig = AssetDatabase.LoadAssetAtPath<AvatarItemConfig>(path);

            if (oldConfig)
            {
                var type = typeof(AvatarItemConfig);
                var dataField =
                    type.GetField(AvatarEditorConstants.FIELD_DATA, BindingFlags.Instance | BindingFlags.NonPublic);
                var oldData = (AvatarItemData)dataField.GetValue(oldConfig);

                var newData = config.Data;
                newData.itemBatch = oldData.itemBatch;

                dataField.SetValue(config, newData);

                var unpurchaseable = type.GetField("unpurchaseable", BindingFlags.Instance | BindingFlags.NonPublic);
                unpurchaseable.SetValue(config, oldConfig.Unpurchaseable);
            }

            if (oldConfig is AvatarApparelConfig apparelConfig)
            {
                var type = typeof(AvatarApparelConfig);
                var clipConfigField =
                    type.GetField("audioClipConfig", BindingFlags.Instance | BindingFlags.NonPublic);
                var clipConfig = clipConfigField.GetValue(oldConfig);

                clipConfigField.SetValue(config, clipConfig);
            }

            AssetDatabase.CreateAsset(config, path);
        }

        private static string[] GetTextureGuids()
        {
            var directories = new[] { AvatarEditorConstants.DIRECTORY_ITEM_TEX };
            var guids = AssetDatabase.FindAssets("t:Texture", directories);
            return guids;
        }

        private static ItemDatabase LoadItemDatabase()
        {
            return AssetDatabase.LoadAssetAtPath<ItemDatabase>(AvatarEditorConstants.PATH_ITEM_DATABASE);
        }
        
        [MenuItem("Tools/Avatar Customization/Database/Clear Old Item Configs")]
        private static void PromptClearOldConfigs()
        {
            var notInDatabasePaths = QueryItemConfigsNotInDatabase().ToArray();
            
            if (!notInDatabasePaths.Any())
            {
                return;
            }

            if (!PromptClearItemsNotInDatabase(notInDatabasePaths))
            {
                return;
            }

            var failedPaths = new List<string>();
            var allDeletionsSuccessful = AssetDatabase.DeleteAssets(notInDatabasePaths, failedPaths);
            
            PromptConfigsCleared(allDeletionsSuccessful);
            
            LogClearOldConfigsOutputToConsole(notInDatabasePaths.Except(failedPaths).ToList(), failedPaths);
        }

        private static bool PromptClearItemsNotInDatabase(IEnumerable<string> notInDatabasePaths)
        {
            var title = "Clear old item configs";
            var message = $"The following AvatarItemConfigs are not in the item database. They are probably old and will be cleared:\n\n{string.Join('\n', notInDatabasePaths.Select(p => Path.GetFileName(p)))}";
            var confirm = "Ok";
            var cancel = "Cancel";
            
            return EditorUtility.DisplayDialog(title, message, confirm, cancel);
        }

        private static void PromptConfigsCleared(bool allDeletionsSuccessful)
        {
            var title = allDeletionsSuccessful ? "All old configs cleared" : "Not all old configs cleared";
            var message = "See the console for a list.";
            var confirm = "Ok";
            
            EditorUtility.DisplayDialog(title, message, confirm);
        }

        private static IEnumerable<string> QueryItemConfigsNotInDatabase()
        {
            var allConfigs = AssetDatabase.FindAssets($"t: {nameof(AvatarItemConfig)}")
                                          .Select(guid => AssetDatabase.GUIDToAssetPath(guid));

            var database = LoadItemDatabase();
            var inDatabase = database.ItemConfigs
                                     .Select(AssetDatabase.GetAssetPath)
                                     .Distinct();

            return allConfigs.Except(inDatabase);
        }
        
        private static void LogClearOldConfigsOutputToConsole(List<string> successfulPaths, List<string> failedPaths)
        {
            var log = new StringBuilder();

            log.Append("Deleted the following item configs:");
            log.AppendLine($"{string.Join('\n', successfulPaths)}");

            if (failedPaths.Any())
            {
                log.AppendLine();
                log.Append("Failed to delete the following item configs:");
                log.AppendLine($"{string.Join('\n', failedPaths)}");
            }
                
            $"{log}".Log(); // Log output to console
        }

        #region Addressables

        private static void PromptPopulateAddressables()
        {
            var title = "Generate Addressables";
            var message = "Configuring Addressable Group settings for generated item data";
            var confirm = "Confirm";

            if (!EditorUtility.DisplayDialog(title, message, confirm))
            {
                return;
            }

            PopulateAddressables();
        }

        private static void PopulateAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var avatarModelGroup = settings.FindGroup(AvatarEditorConstants.ADDR_GROUP_AVATAR_MODELS);
            var avatarTexGroup = settings.FindGroup(AvatarEditorConstants.ADDR_GROUP_AVATAR_TEXTURES);
            var avatarMatsGroup = settings.FindGroup(AvatarEditorConstants.ADDR_GROUP_AVATAR_MATS);
            var avatarIconsGroup = settings.FindGroup(AvatarEditorConstants.ADDR_GROUP_AVATAR_ICONS);
            var avatarConfigsGroup = settings.FindGroup(AvatarEditorConstants.ADDR_GROUP_AVATAR_CONFIGS);

            PopulateAddrGroup(avatarModelGroup, "t:model", AvatarEditorConstants.DIRECTORY_ITEM_MESH);
            PopulateAddrGroup(avatarTexGroup, "t:texture", AvatarEditorConstants.DIRECTORY_ITEM_TEX);
            PopulateAddrGroup(avatarMatsGroup, "t:material", AvatarEditorConstants.DIRECTORY_ITEM_MATS);
            PopulateAddrGroup(avatarIconsGroup, "t:texture", AvatarEditorConstants.DIRECTORY_ITEM_ICONS);

            var guids = AssetDatabase.FindAssets("t:ItemDatabase");
            foreach (var guid in guids)
            {
                CreateAddressable(avatarConfigsGroup, guid);
            }
        }

        private static void PopulateAddrGroup(AddressableAssetGroup addrGroup, string filter, string directory)
        {
            ClearAddressablesGroup(addrGroup);

            var directories = new[] { directory };
            var guids = AssetDatabase.FindAssets(filter, directories);
            var idx = 0;
            foreach (var guid in guids)
            {
                idx++;
                EditorUtility.DisplayProgressBar("Populating Addressable",
                    $"Adding asset {System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(guid))} ... ({idx} / {guids.Length})",
                    (float)idx / guids.Length);
                CreateAddressable(addrGroup, guid);
            }

            EditorUtility.ClearProgressBar();
        }

        private static void CreateAddressable(AddressableAssetGroup addrGroup, string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            var assetEntry = addrGroup.Settings.CreateOrMoveEntry(guid, addrGroup);
            assetEntry.address = asset.name;

            foreach (var label in AvatarEditorConstants.ADDR_LABEL_AVATARS)
            {
                assetEntry.SetLabel(label, true, true);
            }

            EditorUtility.SetDirty(asset);
        }

        private static void ClearAddressablesGroup(AddressableAssetGroup assetGroup)
        {
            var entries = new List<AddressableAssetEntry>(assetGroup.entries);
            foreach (var entry in entries)
            {
                assetGroup.RemoveAssetEntry(entry);
            }
        }

        #endregion
    }
}
