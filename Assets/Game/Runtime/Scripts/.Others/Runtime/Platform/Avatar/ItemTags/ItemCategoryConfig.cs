using UnityEngine;
using UnityEngine.Serialization;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(fileName = "ItemCategoryConfig", menuName = "Config/KumuKulitan/Avatars/ItemTagSelectionConfig")]
    public class ItemCategoryConfig : ScriptableObject
    {
        [SerializeField, HideInInspector, FormerlySerializedAs("itemTag")] private string itemCategory;
        [SerializeField, HideInInspector] private AvatarItemType itemType;
        [SerializeField, HideInInspector] private AvatarItemType deselectedTypes;

        public string ItemCategory => itemCategory;
        public AvatarItemType ItemType => itemType;
        public AvatarItemType DeselectedTypes => deselectedTypes;
    }
}
