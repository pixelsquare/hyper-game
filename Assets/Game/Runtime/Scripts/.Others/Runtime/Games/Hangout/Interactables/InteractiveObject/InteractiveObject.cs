using System;
using Hangout;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public abstract class InteractiveObject : MonoBehaviour
    {
        public enum BroadcastType
        {
            Local,
            Global,
        }

        public enum InteractiveType
        {
            OneShot,
            Looping,
        }

        [SerializeField] private Sprite displayIcon;
        [SerializeField] [HideInInspector] private AssetGuid objectGuid = AssetGuid.NewGuid();

        [Space]
        [SerializeField] protected BroadcastType broadcastType = BroadcastType.Global;

        [SerializeField] protected InteractiveType interactiveType = InteractiveType.OneShot;

        public Sprite DisplayIcon => displayIcon;
        public AssetGuid ObjectGuid => objectGuid;

        public BroadcastType BroadcastMode => broadcastType;

        public abstract void Play();
        public abstract void Stop();

        public virtual void OnInteract(EntityRef slotEntityRef)
        {
            if (broadcastType == BroadcastType.Local)
            {
                Play();
            }
            else if (broadcastType == BroadcastType.Global)
            {
                var playInteractiveCommand = new PlayInteractiveCommand
                {
                    objGuid = objectGuid,
                };

                QuantumRunner.Default.Game.SendCommand(playInteractiveCommand);
            }
        }

        private void HandleOnPlayInteractive(EventOnPlayInteractive eventData)
        {
            if (objectGuid == eventData.objectGuid)
            {
                Play();
            }
        }

        protected virtual void Awake() { }

        protected virtual void OnEnable()
        {
            QuantumEvent.Subscribe<EventOnPlayInteractive>(this, HandleOnPlayInteractive);
        }

        protected virtual void OnDisable()
        {
            QuantumEvent.UnsubscribeListener(this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            var e = Event.current;

            if (e != null && e.commandName.Equals("Duplicate", StringComparison.OrdinalIgnoreCase))
            {
                objectGuid = AssetGuid.NewGuid();
            }
        }
#endif
    }
}
