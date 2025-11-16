using System.Collections.Generic;
using UnityEngine;


namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(menuName = "Scriptable Objects/PartSelectionConfiguration")]
    public class PartSelectionScriptableObject : ScriptableObject
    {
        [SerializeField] private string collectionName = "Parts 1";
        [SerializeField] private PartSelectionIconData newItemsIconData;
        [SerializeField] private PartSelectionIconData ownedItemsIconData;
        [SerializeField] private List<PartSelectionIconData> iconsData = new List<PartSelectionIconData>();

        public List<PartSelectionIconData> IconsData => iconsData;
        public PartSelectionIconData NewItemsIconData => newItemsIconData;
        public PartSelectionIconData OwnedItemsIconData => ownedItemsIconData;
    }
}
