using System.Collections.Generic;

namespace Santelmo.Rinsurv
{
    public interface IState : IRollbackState
    {
        public string StateId { get; }
        public StateMachine Fsm { get; }

        internal bool IsStateEnded { get; set; }
        internal bool IsStateCleanup { get; set; }
        internal List<string> StateTransitionEvents { get; }

        public void OnEnter() { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }
        public void OnLateUpdate() { }
        public void OnExit() { }

        internal void Initialize(string id, StateMachine fsm);
        internal void Reset() { }
    }
}
