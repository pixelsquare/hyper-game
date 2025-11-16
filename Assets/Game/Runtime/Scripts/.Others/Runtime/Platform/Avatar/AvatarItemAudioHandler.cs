using System.Collections.Generic;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Kumu.Kulitan.Avatar
{
    /// <summary>
    /// Handles playing of audio clips tied to character items
    /// </summary>
    public class AvatarItemAudioHandler : AvatarAudioHandler
    {
        [SerializeField] private AudioSource headSource;
        [SerializeField] private AudioSource eyewearSource;
        [SerializeField] private AudioSource faceSource;
        [SerializeField] private AudioSource handsSource;
        [SerializeField] private AudioSource gearSource;

        [SerializeField] private bool enableDebugKeys;

        private HashSet<AssetReference> usedReferences;

        protected override void OnPlayerAvatarLoaded(IEvent<string> callback)
        {
            var playerInitEvent = (PlayerAvatarLoadedEvent)callback;

            if (myView.EntityRef == playerInitEvent.PlayerEntity)
            {
                base.OnPlayerAvatarLoaded(callback);
                var f = QuantumRunner.Default.Game.Frames.Verified;
                var itemList = f.ResolveList(playerInitEvent.AvatarItems.itemStates);

                foreach (var itemState in itemList)
                {
                    var config = ItemDatabase.Current.GetItem(itemState.itemIdx);
                    if (config is AvatarApparelConfig)
                    {
                        var clipConfig = ((AvatarApparelConfig)config).ClipConfig;
                        if (clipConfig.audioReference != null && clipConfig.audioReference.RuntimeKeyIsValid())
                        {
                            clipConfig.typeCode = config.GetTypeCode();
                            usedReferences.Add(clipConfig.audioReference);
                            AudioRefHolder.Instance.AddAudioReference(this, clipConfig);
                        }
                    }
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            usedReferences = new HashSet<AssetReference>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            headSource.Stop();
            eyewearSource.Stop();
            faceSource.Stop();
            handsSource.Stop();
            gearSource.Stop();

            if (AudioRefHolder.Instance != null)
            {
                AudioRefHolder.Instance.RemoveAudioReferenceList(usedReferences);
            }
        }

        private void InitAudioSource(AudioSource aSource, AudioClipConfig clipConfig, AudioClip clip)
        {
            aSource.Stop();
            aSource.clip = clip;

            if (clipConfig.playType == AudioClipConfig.AudioPlayType.LOOP)
            {
                aSource.loop = true;
                aSource.Play();
            }
            else
            {
                aSource.loop = false;
            }
        }

        public override void OnClipReady(AudioClip clip, AudioClipConfig clipConfig)
        {
            switch (clipConfig.typeCode)
            {
                case "HW": //head
                    InitAudioSource(headSource, clipConfig, clip);
                    break;

                case "EW": //eyewear
                    InitAudioSource(eyewearSource, clipConfig, clip);
                    break;

                case "FA": //face
                    InitAudioSource(faceSource, clipConfig, clip);
                    break;

                case "HD": //hands
                    InitAudioSource(handsSource, clipConfig, clip);
                    break;

                case "UA": //upper acc
                    InitAudioSource(gearSource, clipConfig, clip);
                    break;
            }
        }

        private void Update()
        {
            //TODO - inputs below are purely to test out on-demand playing of clips; will remove if functionality is
            //not needed or UI triggers are available
            if (enableDebugKeys && Input.GetKeyUp(KeyCode.G) && headSource.clip != null && !headSource.loop)
            {
                headSource.Stop();
                headSource.Play();
            }

            if (enableDebugKeys && Input.GetKeyUp(KeyCode.H) && eyewearSource.clip != null && !eyewearSource.loop)
            {
                eyewearSource.Stop();
                eyewearSource.Play();
            }

            if (enableDebugKeys && Input.GetKeyUp(KeyCode.J) && faceSource.clip != null && !faceSource.loop)
            {
                faceSource.Stop();
                faceSource.Play();
            }

            if (enableDebugKeys && Input.GetKeyUp(KeyCode.K) && handsSource.clip != null && !handsSource.loop)
            {
                handsSource.Stop();
                handsSource.Play();
            }

            if (enableDebugKeys && Input.GetKeyUp(KeyCode.L) && gearSource.clip != null && !gearSource.loop)
            {
                gearSource.Stop();
                gearSource.Play();
            }
        }
    }
}
