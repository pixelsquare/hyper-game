using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.Avatar
{
    [CreateAssetMenu(fileName = "SwatchTable", menuName = "Config/KumuKulitan/Avatars/SwatchTable")]
    public class SwatchTable : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<AvatarItemType, AvatarSwatchScriptableObject> swatchTable;
        [SerializeField] private AvatarSwatchScriptableObject defaultSwatch;

        public AvatarSwatchScriptableObject DefaultSwatch => defaultSwatch;

        public bool TryGetPalette(AvatarItemType itemType, out AvatarSwatchScriptableObject palette)
        {
            return swatchTable.TryGetValue(itemType, out palette);
        }

        public bool TryGetDefaultColor(string typecode, out Color color)
        {
            var itemType = AvatarItemUtil.ToAvatarItemType(typecode);
            return TryGetDefaultColor(itemType, out color);
        }

        public bool TryGetDefaultColor(AvatarItemType itemType, out Color color)
        {
            if (swatchTable.TryGetValue(itemType, out var swatch))
            {
                color = swatch.Colors[0];
                return true;
            }

            color = Color.white;
            return false;
        }
    }
}
