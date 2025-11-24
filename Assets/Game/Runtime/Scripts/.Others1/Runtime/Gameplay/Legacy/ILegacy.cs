using UnityEngine;

namespace Santelmo.Rinsurv
{
    public interface ILegacy
    {
        public LegacyLayer LegacyLayer { get;  }
        public string LegacyId { get; }
        public string LegacyName { get; }
        public string LegacyDescription { get; }
        public string LegacyIconName { get; }
        public Sprite LegacyTypeSprite { get; }
        public string[] RequiredLegacies { get; }
        public LegacySlot LegacySlot { get; }
        public uint MaxLevel { get; }
        public uint CurrentLevel { get; }

        public void LevelUp();
    }
}
