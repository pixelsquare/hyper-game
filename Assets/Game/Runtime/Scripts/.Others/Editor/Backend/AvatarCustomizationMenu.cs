using System;
using Kumu.Extensions;
using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Common;
using UnityEditor;
using UnityEngine;

namespace Kumu.Kulitan.Backend.Editor
{
    public static class AvatarCustomizationMenu
    {
        [MenuItem("Tools/Avatar Customization/Testing/Clear Prefs Save")]
        public static void ClearSave()
        {
            PlayerPrefs.DeleteKey(MockedServicesUtil.OWNED_ITEMS_KEY);
            PlayerPrefs.DeleteKey(MockedServicesUtil.EQUIPPED_ITEMS_KEY);
        }

        [MenuItem("Tools/Avatar Customization/Testing/Log Prefs Save")]
        public static void CheckSave()
        {
            var equippedItemsJson = PlayerPrefs.GetString(MockedServicesUtil.EQUIPPED_ITEMS_KEY, "{}");
            var equippedItems = JsonHelper.FromJson<AvatarItemState>(equippedItemsJson) ?? Array.Empty<AvatarItemState>();

            foreach (var item in equippedItems)
            {
                Debug.Log($"<color=#00ffff>equipped</color>: {item.itemId.WrapColor(item.hasColor ? item.Color : Color.white)}");
            }

            var ownedItemsJson = PlayerPrefs.GetString(MockedServicesUtil.OWNED_ITEMS_KEY, "{}");
            var ownedItems = JsonHelper.FromJson<AvatarItemState>(ownedItemsJson) ?? Array.Empty<AvatarItemState>();

            foreach (var item in ownedItems)
            {
                Debug.Log($"<color=#ff00ff>owned</color>: {item.itemId.WrapColor(item.hasColor ? item.Color : Color.white)}");
            }
        }

        [MenuItem("Tools/Avatar Customization/Testing/Log Raw Prefs")]
        public static void CheckRawSave()
        {
            var rawEquipped = PlayerPrefs.GetString(MockedServicesUtil.EQUIPPED_ITEMS_KEY, "[]");
            var rawOwned = PlayerPrefs.GetString(MockedServicesUtil.OWNED_ITEMS_KEY, "[]");
            Debug.Log(rawEquipped);
            Debug.Log(rawOwned);
        }
    }
}
