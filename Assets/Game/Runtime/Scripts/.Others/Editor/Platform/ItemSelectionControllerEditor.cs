using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

namespace Kumu.Kulitan.Avatar
{
    [CustomEditor(typeof(ItemSelectionController))]
    public class ItemSelectionControllerEditor : Editor
    {
        private bool isTesting;
        private AvatarItemConfig itemConfig;

        private ItemSelectionController SelectionController => (ItemSelectionController)target;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            if (isTesting = GUILayout.Toggle(isTesting, "Test Mode"))
            {
                itemConfig = (AvatarItemConfig)EditorGUILayout.ObjectField(itemConfig, typeof(AvatarItemConfig), true);
                DrawButton("Select Item", SelectItem);
                DrawButton("Deselect Item", DeselectItem);
                DrawButton("Log Selected Items", LogSelectedItems);
                DrawButton("Log Equipped Items", LogEquippedItems);
                DrawButton("Log Item Color", LogItemColor);
            }
        }

        private void DrawButton(string label, UnityAction action)
        {
            if (GUILayout.Button(label))
            {
                action?.Invoke();
            }
        }

        private void SelectItem()
        {
            SelectionController.SelectItem(itemConfig.Data.itemId);
        }

        private void DeselectItem()
        {
            SelectionController.DeselectItem(itemConfig.Data.itemId);
        }

        private void LogSelectedItems()
        {
            var selectionController = SelectionController;
            var selectedItems = (Dictionary<string, AvatarItemConfig>) selectionController.GetType()
                                                                                                  .GetField("selectedItems", BindingFlags.NonPublic | BindingFlags.Instance)
                                                                                                  .GetValue(selectionController);
            foreach(var pair in selectedItems)
            {
                Debug.Log($"selected item: <color=#{pair.Value.State.colorHex}>{pair.Key}</color>");
            }
        }

        private void LogEquippedItems()
        {
            foreach (var item in UserInventoryData.EquippedItems)
            {
                Debug.Log($"equipped item: <color=#{item.colorHex}>{item.itemId}</color>");
            }
        }

        private void LogItemColor()
        {
            var hasColorLabel = itemConfig.State.hasColor ? "<color=#00ff00>yes</color>" : "<color=#ff000>no</color>";
            Debug.Log($"<color=#{itemConfig.State.colorHex}>{itemConfig.Data.itemId}</color> has color ? {hasColorLabel}");
        }
    }
}
