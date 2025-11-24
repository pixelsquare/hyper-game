using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Santelmo.Rinsurv
{
    using StateMachineEvent = GameEvents.StateMachine;

    public sealed class StateMachine : State, IStateMachine
    {
        public static StateMachineBuilder Builder => new();

        public string Id => _id;
        public bool IsPlaying => _isPlaying;
        public Blackboard Blackboard { get; } = new();

        public IState CurrentState
        {
            get => _currentState;
            private set
            {
                var prevState = _currentState;
                _currentState = value;
                _transitionsMap.TryGetValue(_currentState.StateId, out _stateTransitions);

                _stateTransitions ??= new List<IStateTransition>();
                _stateTransitions.Sort((x, y) => x.Priority.CompareTo(y.Priority));
                _stateTransitions.ToList().ForEach(x => x.Fsm ??= CurrentStateMachine);

                // Batch transitions based on their destination state and type.
                // Conditional and Delegate state transitions with the same destination state
                // will only transition if ALL conditions are met.
                // Other states with the same destination will only transition if ANY conditions are met.
                _stateTransitionBatches = (from transition in _stateTransitions
                                           group transition by new
                                           {
                                               StateName = transition.ToStateName,
                                               Type = transition.GetType()
                                           }
                                           into transitionGroup
                                           select new BatchStateTransition
                                           {
                                               FromStateName = _currentState.StateId,
                                               ToStateName = transitionGroup.Key.StateName,
                                               Priority = transitionGroup.Min(x => x.Priority),
                                               type = transitionGroup.Key.Type,
                                               stateTransitions = transitionGroup
                                                                 .OrderBy(x => x.Priority)
                                                                 .ToList()
                                           }).ToList();

                _stateTransitionBatches.Sort((x, y) => x.Priority.CompareTo(y.Priority));
                StateMachineMono?.Initialize(CurrentStateMachine);

                Dispatcher.SendMessageData(StateMachineEvent.OnStateChangedEvent, new StateChangedEventData
                {
                    StateMachineId = _id,
                    FromStateId = prevState?.StateId,
                    ToStateId = _currentState?.StateId
                });
            }
        }

        public IEnumerable<State> States => from state in _statesMap
                                            select state.Value.Item1?.OfType<State>();
        public IEnumerable<StateTransition> Transitions => (from transition in _transitionsMap
                                                            select transition.Value.OfType<StateTransition>())
                                                            .SelectMany(x => x);

        internal bool IsRootStateMachine => _parentStateMachine == null;

        internal StateMachineMono StateMachineMono
        {
            get
            {
                if (_stateMachineMono != null || !Application.isPlaying)
                {
                    return _stateMachineMono;
                }

                var stateMachineObj = new GameObject($"StateMachineMono_{_id}");
                _stateMachineMono = stateMachineObj.AddComponent<StateMachineMono>();
                stateMachineObj.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
                Object.DontDestroyOnLoad(stateMachineObj);
                return _stateMachineMono;
            }
        }

        internal StateMachine ParentStateMachine => _parentStateMachine as StateMachine;
        internal StateMachine CurrentStateMachine => _currentStateMachine as StateMachine;
        internal DiContainer DiContainer => _diContainer;
        internal IEnumerable<IFSMProvider> Providers => _providers;

        internal bool _isPlaying = true;

        // StateMachine's transition.
        private readonly DiContainer _diContainer;
        private readonly List<IFSMProvider> _providers;
        private readonly IDictionary<string, (IState, Type)> _statesMap;
        private readonly IDictionary<string, List<IStateTransition>> _transitionsMap;

        private string _id;
        private IState _currentState;
        private readonly string _startStateName;
        private StateMachineMono _stateMachineMono;

        // Transitions for the current state.
        private List<IStateTransition> _stateTransitions;
        private List<BatchStateTransition> _stateTransitionBatches = new();

        private IStateMachine _parentStateMachine;
        private readonly IStateMachine _currentStateMachine;

        private StateMachine() { }

        internal StateMachine(StateMachineBuilder builder)
        {
            _id = builder._id;
            _currentStateMachine = this;
            _startStateName = builder._startStateName;
            _isPlaying = builder._playsAutomatically;
            _diContainer = builder._diContainer;
            _providers = builder._providers;
            _statesMap = builder._statesMap;
            _transitionsMap = builder._transitionMap;
            InitializeStartState();
            InitializeProviders();
        }

        public void Play()
        {
            _isPlaying = true;

            // Late start state change.
            // Happens when PlaysAutomatically is set to false.
            InitializeStartState();
        }

        public void Pause()
        {
            _isPlaying = false;
        }

        public void Stop()
        {
            _isPlaying = false;
            ChangeState(_startStateName, true);
        }

        public void Cleanup()
        {
            RemoveStateMachineMono();
            CleanupProviders();
        }

        internal void PollStateCondition()
        {
            if (_stateTransitions == null || _stateTransitions.Count <= 0)
            {
                return;
            }

            foreach (var transition in _stateTransitionBatches)
            {
                if (!transition.OnEvaluate())
                {
                    continue;
                }

                ChangeState(transition.ToStateName);
                break;
            }
        }

        internal void PollStateMachineEndState()
        {
            if (_currentState is not StateMachine stateMachine)
            {
                return;
            }

            var smCurState = stateMachine._currentState;

            if (smCurState == null || !smCurState.IsStateEnded || !smCurState.IsStateCleanup)
            {
                return;
            }

            smCurState.OnExit();
            smCurState.Reset();

            stateMachine._currentState = null;
            stateMachine.RemoveStateMachineMono();
            stateMachine._parentStateMachine = null;
            stateMachine.EndState();
        }

        internal void RemoveStateMachineMono()
        {
            if (Application.isPlaying)
            {
                Object.Destroy(_stateMachineMono?.gameObject);
            }
        }

        internal void ChangeState(string stateName, bool isSilent = false)
        {
            if (_currentState != null && !isSilent)
            {
                _currentState.OnExit();
                _currentState.Reset();
            }

            if (!TryCreateState(stateName, out var nextState))
            {
                throw new ArgumentNullException("State does not exist.", nameof(stateName));
            }

            CurrentState = nextState;

            if (nextState is StateMachine stateMachine)
            {
                stateMachine.Play();
                stateMachine._parentStateMachine ??= _currentStateMachine;
                stateMachine.StateMachineMono?.Initialize(stateMachine.CurrentStateMachine); // Used to instantiate mono.
            }

            if (!isSilent)
            {
                _currentState?.OnEnter();
            }
        }

        private void CleanupProviders()
        {
            _providers.ForEach(x => x.Cleanup());
        }

        private void InitializeProviders()
        {
            _providers.ForEach(x => x.Initialize(this));
        }

        private void InitializeStartState()
        {
            SetStartState(_startStateName);
        }

        private void SetStartState(string startStateName)
        {
            if (string.IsNullOrEmpty(startStateName) 
             || _currentState != null
             || !_isPlaying)
            {
                return;
            }

            ChangeState(startStateName);
        }

        private bool TryCreateState(string stateName, out IState state)
        {
            if (string.IsNullOrEmpty(stateName) || !_statesMap.TryGetValue(stateName, out var stateCache))
            {
                state = null;
                return false;
            }

            var type = stateCache.Item2;
            state = stateCache.Item1;
            state ??= (IState)_diContainer?.TryResolve(type);
            state ??= (IState)Activator.CreateInstance(type);
            state.Initialize(stateName, CurrentStateMachine);
            return true;
        }
    }
}
