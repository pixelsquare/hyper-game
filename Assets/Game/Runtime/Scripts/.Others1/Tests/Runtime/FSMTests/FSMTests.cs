using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Santelmo.Rinsurv.Tests
{
    public class FSMTests
    {
        private const string StateMachineId = "state_machine_id";

        private bool _condition;
        private bool _condition1;
        private bool _condition2;
        private bool _condition3;

        [SetUp]
        public void Setup()
        {
            _condition = false;
            _condition1 = false;
            _condition2 = false;
            _condition3 = false;
        }

        [TearDown]
        public void Cleanup()
        {
        }

        [Test]
        public void FSM_Create()
        {
            var stateMachine = StateMachine.Builder.Build(StateMachineId);
            Assert.That(stateMachine, !Is.Null);
            Assert.That(stateMachine.Id, Is.EqualTo(StateMachineId));
        }

        [UnityTest]
        public IEnumerator FSM_StateTransitionIds()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<BasicTransitionStateA>("MyStateA")
                                           .AddState<BasicTransitionStateB>("MyStateB")
                                           .AddTransition("MyStateA", "MyStateB")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BasicTransitionStateA>());
            Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo("MyStateA"));

            yield return null;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BasicTransitionStateB>());
            Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo("MyStateB"));
        }

        [UnityTest]
        public IEnumerator FSM_EventCallbacks()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<EventCallbackState>("A")
                                           .AddState<PlainState>("B")
                                           .AddTransition("A", "B")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<EventCallbackState>());

            var state = stateMachine.CurrentState.OfType<EventCallbackState>();
            Assert.That(state.stateEvent.HasFlag(EventCallbackState.StateEvents.Enter));

            yield return null;
            yield return null;

            Assert.That(state.stateEvent.HasFlag(EventCallbackState.StateEvents.Update));
            Assert.That(state.stateEvent.HasFlag(EventCallbackState.StateEvents.FixedUpdate));
            Assert.That(state.stateEvent.HasFlag(EventCallbackState.StateEvents.LateUpdate));

            Assert.That(state.stateEvent.HasFlag(EventCallbackState.StateEvents.Exit));
        }

        [UnityTest]
        public IEnumerator FSM_BasicStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<BasicTransitionStateA>("A")
                                           .AddState<BasicTransitionStateB>("B")
                                           .AddTransition("A", "B")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BasicTransitionStateA>());
            yield return null;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BasicTransitionStateB>());
        }

        [UnityTest]
        public IEnumerator FSM_EventStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<EventTransitionStateA>("A")
                                           .AddState<EventTransitionStateB>("B")
                                           .AddTransition("A", "B", "MyEvent")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<EventTransitionStateA>());
            yield return null;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<EventTransitionStateB>());
        }

        [UnityTest]
        public IEnumerator FSM_DelegateStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<DelegateTransitionStateA>("A")
                                           .AddState<DelegateTransitionStateB>("B")
                                           .AddTransition("A", "B", () => _condition)
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<DelegateTransitionStateA>());
            yield return null;

            _condition = true;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<DelegateTransitionStateB>());
        }

        [UnityTest]
        public IEnumerator FSM_CustomConditionalStateTransition()
        {
            var condition = new CustomConditionTransition();
            var stateMachine = StateMachine.Builder
                                           .AddState<CustomConditionStateA>("A")
                                           .AddState<CustomConditionStateA>("B")
                                           .AddTransition("A", "B", condition)
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<CustomConditionStateA>());
            yield return null;

            condition.Condition = true;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<CustomConditionStateA>());
        }

        [UnityTest]
        public IEnumerator FSM_CustomBlackboardConditionalStateTransition()
        {
            var condition = new CustomBlackboardConditionTransition();
            var stateMachine = StateMachine.Builder
                                           .AddState<CustomConditionStateA>("A")
                                           .AddState<CustomConditionStateA>("B")
                                           .AddTransition("A", "B", condition)
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<CustomConditionStateA>());
            yield return null;

            stateMachine.Blackboard.Add<string>("MyEvent", "Hello World");
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<CustomConditionStateA>());
        }

        [UnityTest]
        public IEnumerator FSM_MultipleEventStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<MultipleTransitionStateA>("A")
                                           .AddState<MultipleTransitionStateB>("B")
                                           .AddState<MultipleTransitionStateC>("C")
                                           .AddState<MultipleTransitionStateD>("D")
                                           .AddTransition("A", "B", "MyEvent1")
                                           .AddTransition("A", "C", "MyEvent2")
                                           .AddTransition("A", "D", "MyEvent3")
                                           .AddTransition("C", "A", "MyEvent4")
                                           .AddTransition("C", "C", "MyEvent5")
                                           .AddTransition("C", "D", "MyEvent6")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<MultipleTransitionStateA>());
            yield return null;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<MultipleTransitionStateC>());
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<MultipleTransitionStateD>());
        }

        [UnityTest]
        public IEnumerator FSM_BatchedDelegateStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<BatchedDelegateStateA>("A")
                                           .AddState<BatchedDelegateStateB>("B")
                                           .AddTransition("A", "B", () => _condition1)
                                           .AddTransition("A", "B", () => _condition2)
                                           .AddTransition("A", "B", () => _condition3)
                                           .AddTransition("B", "A", "MyEvent1")
                                           .AddTransition("B", "A", new BatchedDelegateCondition())
                                           .AddTransition("B", "A", "MyEvent3")
                                           .Build(StateMachineId);


            Assert.That(stateMachine.CurrentState, Is.TypeOf<BatchedDelegateStateA>());
            yield return null;

            _condition1 = true;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BatchedDelegateStateA>());

            _condition2 = true;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BatchedDelegateStateA>());

            _condition3 = true;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BatchedDelegateStateB>());

            _condition1 = false;
            _condition2 = false;
            _condition3 = false;

            yield return null;

            stateMachine.Blackboard.Add<string>("MyEvent", "Hello World");
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<BatchedDelegateStateA>());
        }

        [UnityTest]
        public IEnumerator FSM_MultipleEventTriggers()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<MultipleEventTriggerStateA>("A")
                                           .AddState<MultipleEventTriggerStateB>("B")
                                           .AddTransition("A", "B", "MyEvent1")
                                           .AddTransition("A", "B", "MyEvent2")
                                           .AddTransition("A", "B", "MyEvent3")
                                           .AddTransition("B", "A", "MyEvent4")
                                           .AddTransition("B", "A", "MyEvent5")
                                           .AddTransition("B", "A", "MyEvent6")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<MultipleEventTriggerStateA>());
            yield return null;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<MultipleEventTriggerStateB>());
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<MultipleEventTriggerStateA>());
            yield return null;
        }

        [UnityTest]
        public IEnumerator FSM_StateCoroutine()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateCoroutineStateA>("A")
                                           .AddState<StateCoroutineStateB>("B")
                                           .AddTransition("A", "B")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateCoroutineStateA>());
            yield return null;

            var stateA = stateMachine.CurrentState.OfType<StateCoroutineStateA>();
            Assert.That(stateA.Success, Is.True);
            yield return null;
            yield return null;
            yield return null;

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateCoroutineStateB>());
            var stateB = stateMachine.CurrentState.OfType<StateCoroutineStateB>();
            Assert.That(stateB.SampleString, Is.EqualTo("Hello World"));
        }
    }

    #region Event Callbacks

    public class PlainState : State
    {
    }

    public class EventCallbackState : State
    {
        [Flags]
        public enum StateEvents { None, Enter, Exit, Update, FixedUpdate, LateUpdate }

        public StateEvents stateEvent = StateEvents.None;

        public override void OnEnter()
        {
            stateEvent |= StateEvents.Enter;
        }

        public override void OnUpdate()
        {
            stateEvent |= StateEvents.Update;
        }

        public override void OnFixedUpdate()
        {
            stateEvent |= StateEvents.FixedUpdate;
        }

        public override void OnLateUpdate()
        {
            stateEvent |= StateEvents.LateUpdate;
        }

        public override void OnExit()
        {
            stateEvent |= StateEvents.Exit;
        }
    }

    #endregion

    #region Basic State Transition

    public class BasicTransitionStateA : State
    {
        public override void OnUpdate()
        {
            EndState();
        }
    }

    public class BasicTransitionStateB : State
    {
    }

    #endregion

    #region Event State Transition

    public class EventTransitionStateA : State
    {
        public override void OnUpdate()
        {
            EndState("MyEvent");
        }
    }

    public class EventTransitionStateB : State
    {
    }

    #endregion

    #region Delegate State Transition

    public class DelegateTransitionStateA : State
    {
        public override void OnUpdate()
        {
            EndState();
        }
    }

    public class DelegateTransitionStateB : State
    {
    }

    #endregion

    #region Custom Condition State Transition

    public class CustomConditionStateA : State
    {
    }

    public class CustomConditionStateB : State
    {
    }

    public class CustomConditionTransition : ConditionalStateTransition
    {
        public bool Condition { get; set; }

        public override bool OnEvaluate()
        {
            return Condition;
        }
    }

    public class CustomBlackboardConditionTransition : ConditionalStateTransition
    {
        public override bool OnEvaluate()
        {
            return Fsm.Blackboard.ContainsKey("MyEvent");
        }
    }

    #endregion

    #region Multiple State Transition

    public class MultipleTransitionStateA : State
    {
        public override void OnUpdate()
        {
            EndState("MyEvent2");
        }
    }

    public class MultipleTransitionStateB : State
    {
    }

    public class MultipleTransitionStateC : State
    {
        public override void OnUpdate()
        {
            EndState("MyEvent6");
        }
    }

    public class MultipleTransitionStateD : State
    {
    }

    #endregion

    #region Batched Delegate State Transition

    public class BatchedDelegateStateA : State
    {
        public override void OnUpdate()
        {
            EndState();
        }
    }

    public class BatchedDelegateStateB : State
    {
        public override void OnUpdate()
        {
            EndState();
        }
    }

    public class BatchedDelegateCondition : ConditionalStateTransition
    {
        public override bool OnEvaluate()
        {
            return Fsm.Blackboard.ContainsKey("MyEvent");
        }
    }

    #endregion

    #region Multiple Event Triggers

    public class MultipleEventTriggerStateA : State
    {
        public override void OnUpdate()
        {
            EndState("MyEvent3");
        }
    }

    public class MultipleEventTriggerStateB : State
    {
        public override void OnUpdate()
        {
            EndState("MyEvent5");
        }
    }

    #endregion

    #region State Coroutine

    public class StateCoroutineStateA : State
    {
        public bool Success { get; private set; }

        public override void OnEnter()
        {
            StartCoroutine(SampleCoroutine());
        }

        private IEnumerator SampleCoroutine()
        {
            Success = false;
            yield return null;

            Success = true;
            EndState();
        }
    }

    public class StateCoroutineStateB : State
    {
        public string SampleString = "SampleString";

        public override void OnEnter()
        {
            StartCoroutine(SampleCoroutine());
        }

        private IEnumerator SampleCoroutine()
        {
            yield return null;

            SampleString = "Hello World";
        }
    }

    #endregion
}
