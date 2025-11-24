using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Santelmo.Rinsurv.Editor.Tests
{
    [TestFixture]
    public class FSMEditorTests
    {
        private const string StateMachineId = "state_machine_id";
        private const string StateMachineEventKey = "MyEvent";

        private bool _condition = true;

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Cleanup()
        {
            _condition = false;
        }

        [Test]
        public void FSM_Create()
        {
            var stateMachine = StateMachine.Builder.Build(StateMachineId);
            Assert.That(stateMachine, !Is.Null);
            Assert.That(stateMachine.Id, Is.EqualTo(StateMachineId));
        }

        [Test]
        public void FSM_DefaultState()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .Build(StateMachineId);

            Assert.That(stateMachine, !Is.Null);
            Assert.That(stateMachine.Id, Is.EqualTo(StateMachineId));
            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());
            Assert.That(stateMachine.IsPlaying, Is.True);
        }

        [Test]
        public void FSM_AddStates()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddState("C", new StateC())
                                           .Build(StateMachineId);

            Assert.That(stateMachine.States.Count(), Is.EqualTo(3));
        }

        [Test]
        public void FSM_CurrentState()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .SetStartState("A")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());
        }

        [Test]
        public void FSM_StatesProperties()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddState("C", new StateC())
                                           .AddTransition("A", "B")
                                           .AddTransition("B", "C")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());
            Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo("A"));
            Assert.That(stateMachine.CurrentState.Fsm, !Is.Null);

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
            Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo("B"));
            Assert.That(stateMachine.CurrentState.Fsm, !Is.Null);

            state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateC>());
            Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo("C"));
            Assert.That(stateMachine.CurrentState.Fsm, !Is.Null);
        }

        [Test]
        public void FSM_DuplicateStateIdsThrowsException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                StateMachine.Builder
                            .AddState<StateA>("A")
                            .AddState<StateA>("A")
                            .Build(StateMachineId);
            });
        }

        [Test]
        public void FSM_AddTransitions()
        {
            var stateMachine = StateMachine.Builder
                                           .AddTransition("A", "B")
                                           .AddTransition("B", "C", StateMachineEventKey)
                                           .AddTransition("C", "D", () => _condition)
                                           .AddTransition("D", "E", new ConditionalTransition())
                                           .Build(StateMachineId);

            Assert.That(stateMachine.Transitions.Count(), Is.EqualTo(4));
        }

        [Test]
        public void FSM_DuplicateEventTransition()
        {
            Assert.DoesNotThrow(() =>
            {
                StateMachine.Builder
                            .AddState<StateA>("A")
                            .AddState<StateA>("B")
                            .AddTransition("A", "B", StateMachineEventKey)
                            .AddTransition("A", "B", StateMachineEventKey)
                            .Build(StateMachineId);
            });
        }

        [Test]
        public void FSM_DuplicateConditionalTransition()
        {
            Assert.DoesNotThrow(() =>
            {
                StateMachine.Builder
                            .AddState<StateA>("A")
                            .AddState<StateA>("B")
                            .AddTransition("A", "B", () => true)
                            .AddTransition("A", "B", () => false)
                            .AddTransition("A", "B", () => _condition)
                            .Build(StateMachineId);
            });
        }

        [Test]
        public void FSM_SimilarTransitionWithDifferentParams()
        {
            Assert.DoesNotThrow(() =>
            {
                StateMachine.Builder
                            .AddState<StateA>("A")
                            .AddState<StateB>("B")
                            .AddTransition("A", "B")
                            .AddTransition("A", "B", StateMachineEventKey)
                            .AddTransition("A", "B", () => _condition)
                            .Build(StateMachineId);
            });
        }

        [Test]
        public void FSM_BasicStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
        }

        [Test]
        public void FSM_EventStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B", StateMachineEventKey)
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state, StateMachineEventKey);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
        }

        [Test]
        public void FSM_ConditionalStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B", () => _condition)
                                           .AddTransition("A", "B")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            _condition = true;

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state, StateMachineEventKey);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
        }

        [Test]
        public void FSM_CustomConditionalStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B", new ConditionalTransition())
                                           .Build(StateMachineId);
            
            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            stateMachine.Blackboard.Add<string>("MyVariable", "Hello World");

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
        }

        [Test]
        public void FSM_LoopStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B")
                                           .AddTransition("B", "A")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());

            state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());
        }

        [Test]
        public void FSM_CorrectEventKeyStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B", StateMachineEventKey)
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state, StateMachineEventKey);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
        }

        [Test]
        public void FSM_WrongEventKeyStateTransition()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B", StateMachineEventKey)
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state, "WrongEventKey");
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());
        }

        [Test]
        public void FSM_IsPlaying()
        {
            var stateMachine = StateMachine.Builder
                                           .Build(StateMachineId);

            Assert.That(stateMachine.IsPlaying, Is.True);
        }

        [Test]
        public void FSM_Play()
        {
            var stateMachine = StateMachine.Builder
                                           .PlaysAutomatically(false)
                                           .Build(StateMachineId);

            Assert.That(stateMachine.IsPlaying, Is.False);

            stateMachine.Play();
            Assert.That(stateMachine.IsPlaying, Is.True);
        }

        [Test]
        public void FSM_Pause()
        {
            var stateMachine = StateMachine.Builder
                                           .Build(StateMachineId);

            stateMachine.Pause();
            Assert.That(stateMachine.IsPlaying, Is.False);
        }

        [Test]
        public void FSM_Stop()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .Build(StateMachineId);

            stateMachine.Stop();
            Assert.That(stateMachine.IsPlaying, Is.False);
        }

        [Test]
        public void FSM_StateChangedEvent()
        {
            var stateMachine = StateMachine.Builder
                                           .AddState<StateA>("A")
                                           .AddState<StateB>("B")
                                           .AddTransition("A", "B")
                                           .Build(StateMachineId);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateA>());

            StateChangedEventData data = null;

            var handler = new MessageHandler(message =>
            {
                data = (StateChangedEventData)message.Data;
            });

            Dispatcher.AddListener(GameEvents.StateMachine.OnStateChangedEvent, handler, true);

            var state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
            Assert.That(data.StateMachineId, Is.EqualTo(stateMachine.Id));
            Assert.That(data.FromStateId, Is.EqualTo("A"));
            Assert.That(data.ToStateId, Is.EqualTo("B"));
        }

        [Test]
        public void FSM_StateMachineAsState()
        {
            var stateMachineA = StateMachine.Builder
                                            .AddState<StateA>("A")
                                            .AddState<StateB>("B")
                                            .AddTransition("A", "B")
                                            .Build("StateMachineA");

            var stateMachineB = StateMachine.Builder
                                            .AddState<StateA>("AA")
                                            .AddState("BB", stateMachineA)
                                            .AddTransition("AA", "BB")
                                            .Build("StateMachineB");

            var state = stateMachineB.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachineB);

            Assert.That(stateMachineB.CurrentState.StateId, Is.EqualTo("BB"));
            Assert.That(stateMachineB.CurrentState, Is.TypeOf<StateMachine>());

            var stateMachine = stateMachineB.CurrentState.OfType<StateMachine>();

            state = stateMachine.CurrentState.OfType<State>();
            InvokeEndState(state);
            InvokeStateMachinePollUpdate(stateMachine);

            Assert.That(stateMachine.CurrentState, Is.TypeOf<StateB>());
        }

        private void InvokeStateMachinePollUpdate(IStateMachine stateMachine)
        {
            var type = stateMachine.GetType();
            type.GetMethod("PollStateCondition",
                BindingFlags.NonPublic | BindingFlags.Instance,
                Type.DefaultBinder,
                CallingConventions.Any,
                Type.EmptyTypes,
                null)?.Invoke(stateMachine, Array.Empty<object>());
        }

        private void InvokeEndState(State state, string eventName = null)
        {
            var type = state.GetType();
            var types = string.IsNullOrEmpty(eventName) ? new[] { typeof(bool) } : new[] { typeof(string), typeof(bool) };
            var parameters = string.IsNullOrEmpty(eventName) ? new[] { Type.Missing } : new[] { eventName, Type.Missing };
            type.GetMethod("EndState",
                BindingFlags.NonPublic | BindingFlags.Instance,
                Type.DefaultBinder,
                CallingConventions.Any,
                types,
                null)?.Invoke(state, parameters);
        }
    }

    public class StateA : State
    {
    }

    public class StateB : State
    {
    }

    public class StateC : State
    {
    }

    public class ConditionalTransition : ConditionalStateTransition
    {
        public override bool OnEvaluate()
        {
            return Fsm.Blackboard.ContainsKey("MyVariable");
        }
    }
}
