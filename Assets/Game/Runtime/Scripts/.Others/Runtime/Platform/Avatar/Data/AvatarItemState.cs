using System;
using Kumu.Extensions;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Persistent values used for saving Avatar customization data.
    /// </summary>
    [Serializable]
    public struct AvatarItemState
    {
        public string itemId;
        public string typeCode;
        public bool hasColor;
        public string colorHex;

        public Color Color
        {
            get
            {
                if (!ColorUtility.TryParseHtmlString($"#{colorHex}", out var outColor))
                {
                    $"Unable to parse color \"#{colorHex}\" for [{itemId}]. Default will be white.".LogWarning();
                }

                return outColor;
            }
            set => colorHex = ColorUtility.ToHtmlStringRGB(value);
        }

        public AvatarItemType ItemType => AvatarItemUtil.ToAvatarItemType(typeCode);

        public string GetHashString()
        {
            return $"{itemId}{(hasColor ? "1" : "0")}{(hasColor ? colorHex : string.Empty)}";
        }

        public static HangoutItemState ToHangoutItemState(ItemDatabase itemDatabase, AvatarItemState itemState, Color color)
        {
            return new HangoutItemState()
            {
                itemIdx = itemDatabase.GetItemIndex(itemState.itemId),
                r = FP.FromFloat_UNSAFE(color.r),
                g = FP.FromFloat_UNSAFE(color.g),
                b = FP.FromFloat_UNSAFE(color.b),
                a = FP.FromFloat_UNSAFE(color.a),
            };
        }
    }
}
