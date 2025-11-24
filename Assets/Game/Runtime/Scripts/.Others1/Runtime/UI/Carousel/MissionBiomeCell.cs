using FancyCarouselView.Runtime.Scripts;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class MissionBiomeCell : CarouselCell<MissionSelectBiome, MissionBiomeCell>
    {
        [SerializeField] private RinawaImage _backgroundImage;

        protected override void Refresh(MissionSelectBiome itemBiome)
        {
            _backgroundImage.sprite = itemBiome.BackgroundSprite;
        }
    }
}
