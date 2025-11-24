using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

namespace Santelmo.Rinsurv
{
    public sealed class StateMachineBuilder
    {
        internal string _id;
        internal string _startStateName;
        internal bool _playsAutomatically;
        internal DiContainer _diContainer;

        internal List<IFSMProvider> _providers = new();
        internal readonly IDictionary<string, (IState, Type)> _statesMap = new Dictionary<string, (IState, Type)>();
        internal readonly IDictionary<string, List<IStateTransition>> _transitionMap =
                new Dictionary<string, List<IStateTransition>>();

        private int _transitionPriority;

        internal StateMachineBuilder()
        {
            _id = "default_state_machine";
            _startStateName = null;
            _transitionPriority = 0;
            _playsAutomatically = true;
            _diContainer = null;
        }

        public StateMachineBuilder AddState<T>(string stateName) where T : IState
        {
            return AddState(stateName, null, typeof(T));
        }

        public StateMachineBuilder AddState(string stateName, IState state)
        {
            return AddState(stateName, state, state.GetType());
        }

        private StateMachineBuilder AddState(string stateName, IState state, Type stateType)
        {
            _startStateName ??= stateName;

            if (!_statesMap.TryAdd(stateName, new ValueTuple<IState, Type>(state, stateType)))
            {
                throw new ArgumentException("Duplicate State Id.", nameof(stateName));
            }

            return this;
        }

        public StateMachineBuilder AddTransition(string fromState, string toState, StateCondition condition)
        {
            var stateTranstion = new DelegateStateTransition
            {
                FromStateName = fromState,
                ToStateName = toState,
                condition = condition
            };

            return AddTransition(fromState, toState, stateTranstion);
        }

        public StateMachineBuilder AddTransition(string fromState, string toState, string eventName)
        {
            var stateTransition = new EventStateTransition
            {
                FromStateName = fromState,
                ToStateName = toState,
                eventName = eventName
            };

            return AddTransition(fromState, toState, stateTransition);
        }

        public StateMachineBuilder AddTransition(string fromState, string toState, IStateTransition stateTransition = null)
        {
            stateTransition ??= new BasicStateTransition
            {
                FromStateName = fromState,
                ToStateName = toState
            };

            if (stateTransition is ConditionalStateTransition condStateTransition)
            {
                condStateTransition.FromStateName = fromState;
                condStateTransition.ToStateName = toState;
            }

            _transitionPriority++;
            stateTransition.Priority = _transitionPriority;

            if (_transitionMap.TryGetValue(fromState, out var transitionList))
            {
                transitionList.Add(stateTransition);
            }
            else
            {
                _transitionMap.Add(fromState, new List<IStateTransition> { stateTransition });
            }

            return this;
        }

        public StateMachineBuilder AddProvider(params IFSMProvider[] providers)
        {
            _providers.AddRange(providers);
            return this;
        }

        public StateMachineBuilder RemoveProvider(params IFSMProvider[] providers)
        {
            _providers.RemoveAll(x => providers.Any(y => x == y));
            return this;
        }

        public StateMachineBuilder SetStartState(string startStateName)
        {
            _startStateName = startStateName;
            return this;
        }

        public StateMachineBuilder PlaysAutomatically(bool playsAutomatically)
        {
            _playsAutomatically = playsAutomatically;
            return this;
        }

        public IStateMachine Build(string id, DiContainer diContainer = null)
        {
            _id = id;
            _diContainer = diContainer;
            return new StateMachine(this);
        }
    }
}
