using FancyCarouselView.Runtime.Scripts;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class MissionSelectBiome
    {
        public Sprite BackgroundSprite { get; private set; }

        public MissionSelectBiome(Sprite backgroundSprite)
        {
            BackgroundSprite = backgroundSprite;
        }
    }

    public class MissionBiomeView : CarouselView<MissionSelectBiome, MissionBiomeCell>
    {
    }
}
