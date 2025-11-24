using System;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    [Serializable]
    public struct MissionBiome
    {
        [SerializeField] private Sprite _background; // TODO: Convert to asset reference.

        public Sprite Background => _background;
    }
}
