using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public abstract class InputPoller : MonoBehaviour
    {
        protected abstract void PollInput(CallbackPollInput callback);

        protected virtual void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        protected virtual void OnDisable()
        {
            QuantumCallback.UnsubscribeListener(this);
        }
    }
}
