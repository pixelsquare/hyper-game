using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Avatar
{
    public class SimpleAvatarLoader : MonoBehaviour
    {
        [SerializeField] private Transform avatarPivot;
        [SerializeField] private SwatchTable swatchTable;
        [SerializeField] private DefaultItemData defaultItemData;
        [SerializeField] private ItemCategorySelector itemCategorySelector;
        [SerializeField] private SerializableDictionary<string, AvatarItemModelHandleBase> avatarParts = new();

        [SerializeField] private UnityEvent<bool> OnAvatarLoading;

        private ItemDatabase itemDatabase;
        private Vector3 originalLocalPosition;

        private HashSet<AvatarItemState> cachedEquippedItems = new();

        public async void SetEquippedItems(AvatarItemState[] equippedItems)
        {
            OnAvatarLoading?.Invoke(true);

            // Remove item state difference.
            await DiscardRemovedItemConfigs(equippedItems);

            var deselectedItemFlags = AvatarItemType.None;

            // Apply item state changes.
            var newEquippedItems = new HashSet<AvatarItemState>(equippedItems);
            newEquippedItems.ExceptWith(cachedEquippedItems);

            foreach (var item in newEquippedItems)
            {
                if (!itemDatabase.TryGetItem(item.itemId, out var itemConfig))
                {
                    continue;
                }

                var color = item.hasColor ? item.Color : GetAvatarPartColor(itemConfig, Color.white);
                await ApplyItemConfigs(itemConfig, color);

                if (!itemCategorySelector.TryGetValue(itemConfig.Data, out var itemCategory))
                {
                    continue;
                }

                deselectedItemFlags |= itemCategory.DeselectedTypes;
            }

            // Remove deselected item types
            DiscardDeselectedItemTypes(deselectedItemFlags);

            cachedEquippedItems = new HashSet<AvatarItemState>(equippedItems);
            OnAvatarLoading?.Invoke(false);
        }

        public async void SetDefaulItems()
        {
            OnAvatarLoading?.Invoke(true);

            await ClearEquippedItemConfigs();

            foreach (var itemConfig in defaultItemData.AllItems())
            {
                swatchTable.TryGetDefaultColor(itemConfig.GetTypeCode(), out var defaultColor);
                await ApplyItemConfigs(itemConfig, defaultColor);
            }

            OnAvatarLoading?.Invoke(false);
        }

        private async Task ApplyItemConfigs(AvatarItemConfig itemConfig, Color color)
        {
            var typecode = itemConfig.GetTypeCode();

            if (!avatarParts.TryGetValue(typecode, out var avatarPart))
            {
                $"[AvatarLoader] No skinned mesh for {typecode.WrapColor(Color.magenta)}".Log();
                return;
            }

            avatarPart.SetMeshEnabled(true);

            if (itemConfig is IAvatarItemWearable wearable)
            {
                await wearable.WearAvatarItem(avatarPart);
            }

            if (itemConfig is IAvatarItemSameTintable sameTintable)
            {
                var targetTypecode = sameTintable.GetTargetTypeCode();

                if (avatarParts.TryGetValue(targetTypecode, out var targetAvatarPart))
                {
                    sameTintable.TintAvatarItem(avatarPart, targetAvatarPart, color);
                }
            }

            if (itemConfig is IAvatarItemTintable tintable)
            {
                tintable.TintAvatarItem(avatarPart, color);
            }

            if (itemConfig is IAvatarItemOffsetable offsetable)
            {
                avatarPivot.localPosition = originalLocalPosition + offsetable.GetOffset(avatarPart);
            }
        }

        private async Task RemoveItemConfig(AvatarItemConfig itemConfig)
        {
            var typecode = itemConfig.GetTypeCode();

            if (!avatarParts.TryGetValue(typecode, out var avatarPart))
            {
                $"[AvatarLoader] No skinned mesh for {typecode.WrapColor(Color.magenta)}".Log();
                return;
            }

            avatarPart.SetMeshEnabled(false);

            if (itemConfig is IAvatarItemWearable wearable)
            {
                await wearable.RemoveAvatarItem(avatarPart);
            }

            if (itemConfig is IAvatarItemOffsetable offsetable)
            {
                avatarPivot.localPosition = originalLocalPosition;
            }
        }

        private void DiscardDeselectedItemTypes(AvatarItemType deselectedItemFlags)
        {
            // Convert deselected item flags to array.
            var deselectedItemTypes = new HashSet<AvatarItemType>();
            ParseAvatarItemType(deselectedItemFlags, ref deselectedItemTypes);
            deselectedItemTypes.Remove(AvatarItemType.None);

            // Disable deselected item parts.
            foreach (var itemType in deselectedItemTypes)
            {
                var typeCode = AvatarItemUtil.ToTypeCode(itemType);

                if (!avatarParts.TryGetValue(typeCode, out var avatarPart))
                {
                    continue;
                }

                avatarPart.SetMeshEnabled(false);
            }
        }

        private async Task DiscardRemovedItemConfigs(AvatarItemState[] itemStates)
        {
            var removedItems = new HashSet<AvatarItemState>(cachedEquippedItems);
            removedItems.ExceptWith(itemStates);

            foreach (var item in removedItems)
            {
                if (!itemDatabase.TryGetItem(item.itemId, out var itemConfig))
                {
                    continue;
                }

                await RemoveItemConfig(itemConfig);
            }
        }

        private async Task ClearEquippedItemConfigs()
        {
            foreach (var item in cachedEquippedItems)
            {
                if (!itemDatabase.TryGetItem(item.itemId, out var itemConfig))
                {
                    continue;
                }

                await RemoveItemConfig(itemConfig);
            }

            cachedEquippedItems.Clear();
        }

        private void ParseAvatarItemType(AvatarItemType avatarItemTypeFlag, ref HashSet<AvatarItemType> avatarItemTypesList)
        {
            var avatarItemTypes = Enum.GetValues(typeof(AvatarItemType)).Cast<AvatarItemType>();

            foreach (var itemType in avatarItemTypes)
            {
                if (!avatarItemTypeFlag.HasFlag(itemType))
                {
                    continue;
                }

                avatarItemTypesList.Add(itemType);
            }
        }

        private Color GetAvatarPartColor(AvatarItemConfig itemConfig, Color defaultColor)
        {
            if (itemConfig.State.hasColor)
            {
                return itemConfig.State.Color;
            }

            if (swatchTable.TryGetDefaultColor(itemConfig.GetTypeCode(), out var avatarPartColor))
            {
                return avatarPartColor;
            }

            return defaultColor;
        }

        private void Awake()
        {
            originalLocalPosition = avatarPivot.localPosition;
        }

        private async void Start()
        {
            while (itemDatabase == null)
            {
                itemDatabase = ItemDatabase.Current;
                await Task.Yield();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Init Avatar Parts")]
        private void InitAvatarParts()
        {
            foreach (var avatarItemType in Enum.GetValues(typeof(AvatarItemType)).Cast<AvatarItemType>())
            {
                var typeCode = AvatarItemUtil.ToTypeCode(avatarItemType);

                if (avatarParts.ContainsKey(typeCode))
                {
                    continue;
                }

                avatarParts.Add(typeCode, null);
            }
        }
#endif
    }
}
