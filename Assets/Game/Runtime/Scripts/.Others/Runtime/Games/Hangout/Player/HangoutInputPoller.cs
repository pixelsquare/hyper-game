using Kumu.Kulitan.Common;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Input = Quantum.Input;

namespace Kumu.Kulitan.Hangout
{
    public class HangoutInputPoller : InputPoller
    {
        [SerializeField] private Transform playerCamera;
        [SerializeField] private VectorData movementData;
        
        private Vector3 direction;
        private Vector3 cachedForward;

        protected override void PollInput(CallbackPollInput callback)
        {
            var quantumInput = new Input
            {
                Direction = new FPVector2(direction.x.ToFP(), direction.z.ToFP())
            };
            callback.SetInput(quantumInput, DeterministicInputFlags.Repeatable);
        }

        private void Awake()
        {
            movementData.Reset();
        }

        private void Update()
        {
            var input = movementData.Value;
            cachedForward = playerCamera.forward;
            cachedForward.y = 0;
            var moveDir = cachedForward.normalized * input.y + playerCamera.right * input.x;
            direction = moveDir;
        }
    }
}
