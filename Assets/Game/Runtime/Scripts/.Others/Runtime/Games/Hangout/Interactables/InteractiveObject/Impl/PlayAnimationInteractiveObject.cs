using System;
using Hangout;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class PlayAnimationInteractiveObject : InteractiveObject
    {
        [Header("Settings")]
        [SerializeField] private Animator animator;

        [SerializeField] private string animParamKey;
        [SerializeField] private AnimatorControllerParameterType animParamType = AnimatorControllerParameterType.Bool;

        private float clipDuration = 0.0f;

        public override void OnInteract(EntityRef slotEntityRef)
        {
            base.OnInteract(slotEntityRef);
            
            var toggleInteractiveCommand = new ToggleInteractiveCommand
            {
                objGuid = ObjectGuid,
                objEntity = slotEntityRef,
            };
            
            QuantumRunner.Default.Game.SendCommand(toggleInteractiveCommand);            
        }

        public override void Play()
        {
            clipDuration = interactiveType == InteractiveType.OneShot
                    ? GetAnimClipLength()
                    : 0.0f;

            if (interactiveType == InteractiveType.OneShot)
            {
                PlayAnimation();                
            }
            else if (interactiveType == InteractiveType.Looping)
            {
                ToggleAnimation();
            }
        }

        public override void Stop()
        {
            StopAnimation();
        }

        private void PlayAnimation()
        {
            switch (animParamType)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(animParamKey, true);
                    break;

                default: throw new ArgumentOutOfRangeException("animParamType", "Must be of type boolean.");
            }
        }

        private void StopAnimation()
        {
            switch (animParamType)
            {
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(animParamKey, false);
                    break;

                default: throw new ArgumentOutOfRangeException("animParamType", "Must be of type boolean.");
            }
        }

        private void ToggleAnimation()
        {
            switch (animParamType)
            {
                case AnimatorControllerParameterType.Bool:
                    var value = animator.GetBool(animParamKey);
                    animator.SetBool(animParamKey, !value);
                    break;

                default: throw new ArgumentOutOfRangeException("animParamType", "Must be of type boolean.");
            }
        }

        private float GetAnimClipLength()
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            Debug.Assert(clips.Length <= 1, $"There are more than one animation clips. Will be using {clips[0].name}");
            return clips.Length == 1 ? clips[0].length : 0.0f;
        }

        protected override void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
                Debug.Assert(animator != null, "Animator not found! Please assign it.");
            }
        }

        private void Update()
        {
            if (clipDuration > 0.0f)
            {
                clipDuration -= Time.deltaTime;

                if (clipDuration <= 0.0f)
                {
                    Stop();
                }
            }
        }
    }
}
