using System.Threading.Tasks;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using UnityEngine;
using UnityEngine.Events;

namespace Kumu.Kulitan.Avatar
{
    public class AvatarCustomizer : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<string, AvatarItemModelHandleBase> characterParts;
        [SerializeField] private SwatchTable swatchTable;
        [SerializeField] private Transform floorPivot;
        [SerializeField] private AvatarColorPicker colorPicker; //todo: [jef] refactor; decouple this
        [SerializeField] private UnityEvent<bool> onLoading;

        private Vector3 originLocalPosition;

        public async Task SelectItem(AvatarItemConfig config)
        {
            var color = GetColor(config);
            await ProcessItem(config, color, true);
        }

        public async Task SelectItem(AvatarItemConfig config, Color color)
        {
            await ProcessItem(config, color, true);
        }

        /// <summary>
        /// Called by ItemSelection event.
        /// </summary>
        /// <param name="data">The AvatarItem Data</param>
        /// <param name="isSelected">checks if item is selected/deselected</param>
        public async Task OnItemSelected(AvatarItemConfig config, bool isSelected)
        {
            if (isSelected)
            {
                onLoading.Invoke(true);
            }

            var color = GetColor(config);
            await ProcessItem(config, color, isSelected);

            if (isSelected)
            {
                onLoading.Invoke(false);
            }
        }

        public void DeselectAll()
        {
            foreach (var pair in characterParts)
            {
                var part = pair.Value;
                part.SetMeshEnabled(false);
            }
        }

        private async Task ProcessItem(AvatarItemConfig config, Color color, bool isSelected)
        {
            if (!characterParts.TryGetValue(config.GetTypeCode(), out var avatarColorizer))
            {
                $"No skinned mesh for {config.GetTypeCode().WrapColor(Color.magenta)}.".LogError();
                return;
            }

            avatarColorizer.SetMeshEnabled(isSelected);

            if (config is IAvatarItemWearable wearable)
            {
                if (isSelected)
                {
                    await wearable.WearAvatarItem(avatarColorizer);
                }
                else
                {
                    await wearable.RemoveAvatarItem(avatarColorizer);
                }
            }

            if (config is IAvatarItemSameTintable sameTintable
                && isSelected)
            {
                var targetTypecode = sameTintable.GetTargetTypeCode();

                if (characterParts.TryGetValue(targetTypecode, out var targetColorizer))
                {
                    sameTintable.TintAvatarItem(avatarColorizer, targetColorizer, color);
                }
            }

            if (config is IAvatarItemTintable tintable
                && isSelected)
            {
                tintable.TintAvatarItem(avatarColorizer, color);
                if (colorPicker)
                {
                    colorPicker.TryPickColor(color);
                }
            }

            if (config is IAvatarItemOffsetable offsetable)
            {
                if (isSelected)
                {
                    floorPivot.localPosition = originLocalPosition + offsetable.GetOffset(avatarColorizer);
                }
                else
                {
                    floorPivot.localPosition = originLocalPosition;
                }
            }
        }

        private Color GetColor(AvatarItemConfig config)
        {
            if (config.State.hasColor)
            {
                return config.State.Color;
            }

            var itemType = AvatarItemUtil.ToAvatarItemType(config.GetTypeCode());
            if (swatchTable.TryGetDefaultColor(itemType, out var defaultColor))
            {
                return defaultColor;
            }

            return Color.white;
        }

        private void Awake()
        {
            originLocalPosition = floorPivot.localPosition;
        }
    }
}
