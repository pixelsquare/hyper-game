using UnityEngine;
using UnityEditor;

namespace Kumu.Kulitan.Avatar
{
    [CustomEditor(typeof(ItemSelection))]
    public class ItemSelectionEditor : Editor
    {
        private ItemSelection ItemSelection => (ItemSelection)target;

        private bool isTesting;
        private AvatarItemType itemType;
        private string itemTag;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            isTesting = GUILayout.Toggle(isTesting, "Test Mode");
            if (!isTesting)
            {
                return;
            }

            itemType = (AvatarItemType) EditorGUILayout.EnumFlagsField("Item Type", itemType);
            itemTag = EditorGUILayout.TextField("Item Tag", itemTag);

            if (GUILayout.Button("Filter"))
            {
                ItemSelection.FilterItems(itemType);
            }

            if (GUILayout.Button("Filter Owned"))
            {
                ItemSelection.FilterItemsByOwned(itemType);
            }

            if (GUILayout.Button("Filter New"))
            {
                ItemSelection.FilterItemsByNew(itemType);
            }
        }
    }
}
