using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Santelmo.Rinsurv
{
    public abstract class State : IState
    {
        public string StateId { get; private set; }
        public StateMachine Fsm { get; private set; }

        bool IState.IsStateEnded
        {
            get => _stateEnded;
            set => _stateEnded = value;
        }

        bool IState.IsStateCleanup
        {
            get => _stateCleanup;
            set => _stateCleanup = this is StateMachine && value;
        }

        List<string> IState.StateTransitionEvents => _stateTransitionEvents;

        private bool _stateEnded;
        private bool _stateCleanup;
        private readonly List<string> _stateTransitionEvents = new();

        public virtual void OnEnter()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnLateUpdate()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual UniTask<bool> OnRollbackAsync()
        {
            return UniTask.FromResult(true);
        }

        void IState.Initialize(string id, StateMachine sm)
        {
            Fsm = sm;
            StateId = id;
        }

        void IState.Reset()
        {
            _stateEnded = false;
            _stateCleanup = false;
            _stateTransitionEvents.Clear();
        }

        /// <param name="cleanup">
        ///     If this state is a StateMachine.
        ///     Setting cleanup to true will end its state from the parent state machine.
        /// </param>
        protected void EndState(bool cleanup = false)
        {
            _stateEnded = true;
            _stateCleanup = cleanup;

            if (Fsm is { IsRootStateMachine: false })
            {
                Fsm.EndState(cleanup);
            }
        }

        protected void EndState(string eventName, bool cleanup = false)
        {
            _stateEnded = true;
            _stateCleanup = cleanup;
            _stateTransitionEvents.Add(eventName);

            if (Fsm is { IsRootStateMachine: false })
            {
                Fsm?.EndState(eventName, cleanup);
            }
        }

        protected void StartCoroutine(IEnumerator coroutine)
        {
            if (Fsm == null || Fsm.StateMachineMono == null)
            {
                throw new NullReferenceException("State Machine Mono does not exist.");
            }

            Fsm?.StateMachineMono.StartCoroutine(coroutine);
        }
    }
}
