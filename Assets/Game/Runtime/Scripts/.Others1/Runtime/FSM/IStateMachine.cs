using System.Collections.Generic;

namespace Santelmo.Rinsurv
{
    public interface IStateMachine : IState
    {
        public string Id { get; }
        public bool IsPlaying { get; }

        public IState CurrentState { get; }
        public Blackboard Blackboard { get; }

        public IEnumerable<State> States { get; }
        public IEnumerable<StateTransition> Transitions { get; }

        public void Play();
        public void Pause();
        public void Stop();
        public void Cleanup();
    }
}
