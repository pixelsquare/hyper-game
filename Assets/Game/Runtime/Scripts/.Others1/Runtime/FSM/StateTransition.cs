using System;
using System.Collections.Generic;
using System.Linq;

namespace Santelmo.Rinsurv
{
    public delegate bool StateCondition();

    public abstract class StateTransition : IStateTransition
    {
        public string FromStateName { get; internal set; }
        public string ToStateName { get; internal set; }
        public int Priority { get; set; }
        protected StateMachine Fsm { get; private set; }

        StateMachine IStateTransition.Fsm
        {
            get => Fsm;
            set => Fsm = value;
        }

        public abstract bool OnEvaluate();
    }

    public class ConditionalStateTransition : StateTransition
    {
        public override bool OnEvaluate() { return false; }
    }

    public class DelegateStateTransition : StateTransition
    {
        public StateCondition condition;

        public override bool OnEvaluate()
        {
            return condition?.Invoke() ?? false;
        }
    }

    public class EventStateTransition : StateTransition
    {
        public string eventName;

        public override bool OnEvaluate()
        {
            var currentState = Fsm.CurrentState;
            return currentState.IsStateEnded && currentState.StateTransitionEvents.Contains(eventName);
        }
    }

    public class BasicStateTransition : StateTransition
    {
        public override bool OnEvaluate()
        {
            return Fsm.CurrentState.IsStateEnded;
        }
    }

    internal class BatchStateTransition : StateTransition
    {
        public Type type;
        public List<IStateTransition> stateTransitions;

        public override bool OnEvaluate()
        {
            if (type == typeof(ConditionalStateTransition) || type == typeof(DelegateStateTransition))
            {
                return stateTransitions.All(x => x.OnEvaluate());
            }

            return stateTransitions.Any(x => x.OnEvaluate());
        }
    }
}
