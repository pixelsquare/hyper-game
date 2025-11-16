using Hangout;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class MinigameCountdownInteractiveObject : InteractiveObject
    {
        [SerializeField] private EntityView entity;
        
        public override void Play()
        {
            var toggleInteractiveCommand = new MinigameCountdownCommand
            {
                buttonEntity = entity.EntityRef,
            };
            
            QuantumRunner.Default.Game.SendCommand(toggleInteractiveCommand);
        }

        public override void Stop()
        {
        }
    }
}
