using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Quantum;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Hangout
{
    public class CharacterAnimator : MonoBehaviour
    {
        private static readonly int movement = Animator.StringToHash("speed");

        [SerializeField] private EntityView entityView;

#if !ADDRESSABLES_ENABLED
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private AnimationConfig animationConfig;
#else
        [SerializeField] private AssetReferenceT<AnimationConfig> animationConfigRef;
        private AnimationConfig animationConfig;
        private Animator characterAnimator;
#endif

        private long curAnimGuid;
        private EntityRef entityRef;
        private bool isPolling;

        public void Initialize()
        {
            entityRef = entityView.EntityRef;
            isPolling = true;
        }

        public void DeInitialize()
        {
            isPolling = false;
        }

#if ADDRESSABLES_ENABLED
        public void InitializeModel(GameObject modelObj)
        {
            characterAnimator = modelObj.GetComponentInChildren<Animator>();
        }
#endif

        public void PlayAnimation(AnimationDataAsset animationData)
        {
            switch (animationData.AnimParamType)
            {
                case AnimatorControllerParameterType.Bool:
                    characterAnimator.SetBool(animationData.AnimParamKey, true);
                    characterAnimator.Play($"{animationData.DisplayName}", 0, 0.0f);
                    break;
                default:
                    $"{animationData.AnimParamType} is not yet handled".LogError();
                    break;
            }

            if (animationData is EmotesAnimationAsset emoteAnimAsset && emoteAnimAsset.EmoteBubbleAnimation != null)
            {
                GlobalNotifier.Instance.Trigger(new EmoteBubblePlayedEvent(entityRef, emoteAnimAsset));
            }
        }

        public void ClearAnimations()
        {
            var animLen = animationConfig.CharacterAnimations.Count;

            for (var i = 0; i < animLen; i++)
            {
                var animData = animationConfig.CharacterAnimations[i];

                switch (animData.AnimParamType)
                {
                    case AnimatorControllerParameterType.Bool:
                        characterAnimator.SetBool(animData.AnimParamKey, false);
                        break;
                    default:
                        $"{animData.AnimParamType} is not yet handled".LogError();
                        break;
                }
            }

            characterAnimator.WriteDefaultValues();
        }

#if ADDRESSABLES_ENABLED
        private async void InitializeAnimationConfig()
        {
            animationConfig = await animationConfigRef.LoadAssetAsync<AnimationConfig>().Task;
        }
#endif
        
        private void PollAnimation()
        {
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var playerAnim = f.Get<PlayerAnimation>(entityRef);

            characterAnimator.SetFloat(movement, playerAnim.moveSpeed.AsFloat);

            if (playerAnim.curAnimGuid.id != curAnimGuid)
            {
                curAnimGuid = playerAnim.curAnimGuid.id;

                if (curAnimGuid == 0)
                {
                    ClearAnimations();
                    GlobalNotifier.Instance.Trigger(new OnPlayerEmoteEvent(entityRef, null, false));
                    return;
                }

                var animData = UnityDB.FindAsset<AnimationDataAsset>(playerAnim.curAnimGuid.id);

                if (animData != null)
                {
                    ClearAnimations();
                    PlayAnimation(animData);
                    GlobalNotifier.Instance.Trigger(new OnPlayerEmoteEvent(entityRef, animData, true));
                }
                else
                {
                    $"CurrentAnimationState: {playerAnim.curAnimGuid.id} has no mapping. No animation will be played!".LogError();
                }
            }
        }

        private void Start()
        {
#if ADDRESSABLES_ENABLED
            InitializeAnimationConfig();
#endif
        }

        private void OnDestroy()
        {
#if ADDRESSABLES_ENABLED
            animationConfigRef.ReleaseAsset();
#endif
        }

        private void Update()
        {
            if (!isPolling || QuantumRunner.Default == null || characterAnimator == null)
            {
                return;
            }

            PollAnimation();
        }
    }
}
