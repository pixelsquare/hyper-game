using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace Kumu.Kulitan.Hangout 
{
    public class AudioLinkStateBehaviour : StateMachineBehaviour
    {
        [SerializeField] private TimingMapping[] timingMappings;
        [SerializeField] private bool playOnce;
        private int nextSoundClipIndex;
        private int nextTimingIndex;
        private Transform _actorTrans;
        private AnimationLinkedAudio linkedAudioHandler;
        private int loopCounter;
        private HashSet<AudioSource> activeSources;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Init(animator);
            nextTimingIndex = 0;
            loopCounter = 0;
            activeSources = new HashSet<AudioSource>();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Init(animator); //failsafe, should already have been populated on state enter

            var animRepetition = (int)stateInfo.normalizedTime;
            var animTime = stateInfo.normalizedTime - animRepetition;

            if (nextTimingIndex < timingMappings.Length)
            {
                var nextTiming = timingMappings[nextTimingIndex].timing;

                if (animTime >= nextTiming)
                {
                    if (timingMappings[nextTimingIndex].overrideClipReferences.Length > 0)
                    {
                        var source =linkedAudioHandler.PlayAudioByClip(timingMappings[nextTimingIndex].GetNextAudioClipByReference(), timingMappings[nextTimingIndex].volumeOverride);
                        if (source != null)
                        {
                            activeSources.Add(source);
                        }
                    }
                    else if (timingMappings[nextTimingIndex].overrideClips.Length > 0)
                    {
                        var source =linkedAudioHandler.PlayAudioByClip(timingMappings[nextTimingIndex].GetNextAudioClip(), timingMappings[nextTimingIndex].volumeOverride);
                        if (source != null)
                        {
                            activeSources.Add(source);
                        }
                    }

                    nextTimingIndex += 1;
                }
            }

            if (playOnce)
            {
                return;
            }

            if (Mathf.FloorToInt(stateInfo.normalizedTime) <= loopCounter)
            {
                return;
            }

            loopCounter++;
            nextTimingIndex = 0;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            loopCounter = 0;
            nextTimingIndex = 0;

            foreach (var source in activeSources)
            {
                source.Stop();
            }

            activeSources.Clear();
        }

        private void Init(Animator anim)
        {
            if (linkedAudioHandler != null)
            {
                return;
            }

            linkedAudioHandler = anim.transform.gameObject.GetComponentInParent<AnimationLinkedAudio>();
        }

        [Serializable]
        public class TimingMapping
        {
            public float timing;
            public bool playRandomOverrideClip;
            public AssetReference[] overrideClipReferences;
            public AudioClip[] overrideClips;
            public float volumeOverride = 1f;
            [NonSerialized] private int nextSoundClipIndex;

            public AudioClip GetNextAudioClipByReference()
            {
                if (overrideClipReferences.Length <= 0)
                {
                    return null;
                }

                var clipToUse = AudioRefHolder.Instance.GetAudioClip(overrideClipReferences[nextSoundClipIndex]);

                if (playRandomOverrideClip)
                {
                    clipToUse = AudioRefHolder.Instance.GetAudioClip(overrideClipReferences[Random.Range(0, overrideClips.Length)]);
                }
                else
                {
                    clipToUse = AudioRefHolder.Instance.GetAudioClip(overrideClipReferences[nextSoundClipIndex]);
                }

                nextSoundClipIndex += 1;
                if (nextSoundClipIndex >= overrideClips.Length)
                {
                    nextSoundClipIndex = 0;
                }

                return clipToUse;
            }
            
            public AudioClip GetNextAudioClip()
            {
                if (overrideClips.Length <= 0)
                {
                    return null;
                }

                var clipToUse = overrideClips[nextSoundClipIndex];

                if (playRandomOverrideClip)
                {
                    clipToUse = overrideClips[Random.Range(0, overrideClips.Length)];
                }

                nextSoundClipIndex += 1;
                if (nextSoundClipIndex >= overrideClips.Length)
                {
                    nextSoundClipIndex = 0;
                }

                return clipToUse;
            }
        }
    }
}
