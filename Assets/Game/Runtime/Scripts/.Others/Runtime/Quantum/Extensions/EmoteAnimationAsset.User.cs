using UnityEngine;
using Spine.Unity;

public partial class EmotesAnimationAsset
{
    [SerializeField] private AnimationReferenceAsset emoteBubbleAnimation;

    public AnimationReferenceAsset EmoteBubbleAnimation => emoteBubbleAnimation;
}
