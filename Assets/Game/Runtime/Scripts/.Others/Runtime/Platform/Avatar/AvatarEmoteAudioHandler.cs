using System.Collections.Generic;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Handles loading of audio clips tied to emotes
    /// </summary>
    public class AvatarEmoteAudioHandler : AvatarAudioHandler
    {
#if !ADDRESSABLES_ENABLED
        [SerializeField] private AnimationConfig animConfig;
#else
        [SerializeField] private AssetReferenceT<AnimationConfig> animConfigRef;
        private AnimationConfig animConfig;
#endif

        private HashSet<AssetReference> usedReferences;

        public override void OnClipReady(AudioClip clip, AudioClipConfig clipConfig)
        {
        }

        protected override void OnPlayerAvatarLoaded(IEvent<string> callback)
        {
            var playerInitEvent = (PlayerAvatarLoadedEvent)callback;

            if (myView.EntityRef == playerInitEvent.PlayerEntity)
            {
                base.OnPlayerAvatarLoaded(callback);

                var characterAnims = animConfig.CharacterAnimations;

                foreach (var charAnim in characterAnims)
                {
                    if (charAnim.ClipConfig.audioReference != null &&
                        charAnim.ClipConfig.audioReference.RuntimeKeyIsValid())
                    {
                        usedReferences.Add(charAnim.ClipConfig.audioReference);
                        AudioRefHolder.Instance.AddAudioReference(this, charAnim.ClipConfig);
                    }
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            usedReferences = new HashSet<AssetReference>();

#if ADDRESSABLES_ENABLED
            InitializeAnimationConfig();
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (AudioRefHolder.Instance != null)
            {
                AudioRefHolder.Instance.RemoveAudioReferenceList(usedReferences);
            }

#if ADDRESSABLES_ENABLED
            animConfigRef.ReleaseAsset();
#endif
        }

#if ADDRESSABLES_ENABLED
        private async void InitializeAnimationConfig()
        {
            animConfig = await animConfigRef.LoadAssetAsync<AnimationConfig>().Task;
        }
#endif
    }
}
