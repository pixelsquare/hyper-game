using Kumu.Kulitan.Common;
using Spine.Unity;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class EmoteBubbleUI : MonoBehaviour
    {
        [SerializeField] private ObjectUIFollower objectUIFollower;
        [SerializeField] private SkeletonGraphic emoteBubble;

        public void Initialize(Transform playerTransform, Camera camera)
        {
            objectUIFollower.SetCamera(camera);
            objectUIFollower.InitializeTarget(playerTransform);
        }

        public void PlayAnimation(AnimationReferenceAsset animationRef)
        {
            emoteBubble.gameObject.SetActive(true);
            emoteBubble.AnimationState.SetAnimation(0, animationRef, false);
            emoteBubble.AnimationState.Complete += entry => emoteBubble.gameObject.SetActive(false);
        }
    }
}
