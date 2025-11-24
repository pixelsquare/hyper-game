using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class InventoryHud : BaseHud
    {
        [SerializeField] private InventoryCategoryType _initialCategoryType = InventoryCategoryType.HeroShard;
        [SerializeField] private EquipmentRarity _initialRarityFilter = EquipmentRarity.Any;
        [SerializeField] private RinawaDropdown _rarityDropdown;

        public InventoryCategoryType InitialCategoryType => _initialCategoryType;

        [Inject] private IAudioManager _audioManager;
        [Inject] private ItemDatabase _itemDatabase;
        [Inject] private IInventoryModule _inventoryModule;
        [Inject] private InventoryItemPopulator _inventoryItemPopulator;

        private InventoryCategoryType _category;
        private EquipmentRarity _rarity;
        private List<ValueTuple<string, int>> _rarityValueFlags;

        private readonly Dictionary<InventoryCategoryType, IEnumerable<IItem>> _itemCategoryMap = new();

        public void GenerateCategoryItems(InventoryCategoryType category)
        {
            _category = category;
            GenerateItems(_category, _rarity);
        }

        private IEnumerable<IItem> GetCategoryItems(InventoryCategoryType category)
        {
            if (_itemCategoryMap.TryGetValue(category, out var categoryItems))
            {
                return categoryItems.Where(x => _inventoryModule.GetItemCount(x) > 0);
            }

            categoryItems = category switch
            {
                InventoryCategoryType.HeroShard       => _itemDatabase.GetItems<Shard>(),
                InventoryCategoryType.Weapon          => _itemDatabase.GetItems<Weapon>(),
                InventoryCategoryType.EmblemChange    => _itemDatabase.GetItems<EmblemChange>(),
                InventoryCategoryType.EmblemDeparture => _itemDatabase.GetItems<EmblemDeparture>(),
                InventoryCategoryType.EmblemPursuit   => _itemDatabase.GetItems<EmblemPursuit>(),
                InventoryCategoryType.Relic           => _itemDatabase.GetItems<Relic>(),
                _                                     => null
            };

            categoryItems = categoryItems?.Where(x => _inventoryModule.GetItemCount(x) > 0); // Use when cloud sync is enabled.
            _itemCategoryMap[category] = categoryItems;
            return categoryItems;
        }

        private IEnumerable<IItem> GetFilteredItems(IEnumerable<IItem> items, EquipmentRarity rarity)
        {
            if (rarity == EquipmentRarity.Any)
            {
                return items;
            }

            return items.OfType<IEquipment>()
                        .Where(x => rarity.HasFlag(x.Rarity))
                        .Select(x => x);
        }

        private void GenerateItems(InventoryCategoryType filter, EquipmentRarity rarity)
        {
            var items = GetCategoryItems(filter);
            items = GetFilteredItems(items, rarity);
            _inventoryItemPopulator.GenerateItems(items);
        }

        private void PopulateRarityDropdown()
        {
            var dropdownOps = Enum.GetNames(typeof(EquipmentRarity)).ToList();
            dropdownOps.Remove(nameof(EquipmentRarity.None));

            // Move `Any` on the top of the list.
            var lastIndex = dropdownOps.Count - 1;
            var lastElement = dropdownOps[lastIndex];
            dropdownOps.RemoveAt(lastIndex);
            dropdownOps.Insert(0, lastElement);

            _rarityDropdown.ClearOptions();
            _rarityDropdown.AddOptions(dropdownOps);

            // Store rarity enum string and value
            _rarityValueFlags = dropdownOps.Select(x => new ValueTuple<string, int>(x, (int)Enum.Parse<EquipmentRarity>(x))).ToList();
            _rarityDropdown.value = _rarityValueFlags.IndexOf(new ValueTuple<string, int>(_initialRarityFilter.ToString(), (int)_initialRarityFilter));
        }

        private void HandleDropdownEvent(int dropdownValue)
        {
            _rarity = (EquipmentRarity)_rarityValueFlags[dropdownValue].Item2;
            GenerateItems(_category, _rarity);
        }

        private void OnEnable()
        {
            _audioManager.PlaySound(Bgm.Inventory);
            _rarityDropdown.onValueChanged.AddListener(HandleDropdownEvent);
        }

        private void OnDisable()
        {
            _rarityDropdown.onValueChanged.RemoveListener(HandleDropdownEvent);
        }

        private void Start()
        {
            _category = _initialCategoryType;
            _rarity = _initialRarityFilter;

            GenerateItems(_category, _rarity);
            PopulateRarityDropdown();
        }
    }
}
