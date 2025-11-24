using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Santelmo.Rinsurv
{
    using StateMachineEvent = GameEvents.StateMachine;
    using GameStateName = GameConstants.GameStates;
    using AppStateEvent = GameEvents.AppState;

    public class AndroidOEMProvider : IFSMProvider
    {
        public int RollbackStackCount => _rollbackStateStack.Count;

        private bool _didRollback;
        private IStateMachine _stateMachine;

        private readonly Dictionary<string, IStateMachine> _stateMachineMap = new();
        private readonly Stack<StateChangedEventData> _rollbackStateStack = new();

        public void Initialize(IStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _stateMachineMap[stateMachine.Id] = stateMachine;
            Dispatcher.AddListener(StateMachineEvent.OnStateChangedEvent, HandleStateMachineChanged);
            Dispatcher.AddListener(AppStateEvent.ToPreviousScreenEvent, HandleGoToPreviousStateEvent);
            Observable.EveryUpdate()
#if !UNITY_EDITOR && UNITY_ANDROID
                      .Where(_ => Input.GetKeyDown(KeyCode.Escape))
#else
                      .Where(_ => Input.GetKeyDown(KeyCode.B))
#endif
                      .Subscribe(x => GoToPreviousState());
        }

        public void Cleanup()
        {
            Dispatcher.RemoveListener(StateMachineEvent.OnStateChangedEvent, HandleStateMachineChanged, true);
            Dispatcher.RemoveListener(AppStateEvent.ToPreviousScreenEvent, HandleGoToPreviousStateEvent, true);
        }

        private async void GoToPreviousState()
        {
            StateChangedEventData stateChangedEventData = null;

            // Look for any rollback state with valid from state id.
            // Empty string usually happens when a state is state machine
            while (_rollbackStateStack.Count > 0)
            {
                if (_rollbackStateStack.TryPop(out stateChangedEventData)
                 && !string.IsNullOrEmpty(stateChangedEventData.FromStateId))
                {
                    break;
                }
            }

            if (!await OnCurrentStateRollback())
            {
                return;
            }

            if (stateChangedEventData == null
             || !_stateMachineMap.TryGetValue(stateChangedEventData.StateMachineId, out var iStateMachine)
             || iStateMachine is not StateMachine stateMachine)
            {
                return;
            }

            _didRollback = true;
            stateMachine.ChangeState(stateChangedEventData.FromStateId);
        }

        private async UniTask<bool> OnCurrentStateRollback()
        {
            if (_stateMachine.CurrentState is StateMachine curState)
            {
                return await curState.CurrentState.OnRollbackAsync();
            }

            return await _stateMachine.CurrentState.OnRollbackAsync();
        }

        private void HandleStateMachineChanged(IMessage message)
        {
            if (_didRollback)
            {
                _didRollback = !_didRollback;
                return;
            }

            if (message.Data is not StateChangedEventData eventData
             || string.IsNullOrEmpty(eventData.FromStateId)
             || eventData.FromStateId.Equals(GameStateName.BootState)
             || eventData.FromStateId.Equals(GameStateName.LoginState)
             || eventData.FromStateId.Equals(GameStateName.AssetLoaderState)
             || eventData.FromStateId.Equals(GameStateName.LoadGameState)
             || eventData.FromStateId.Equals(GameStateName.UnloadGameState)
             || eventData.FromStateId.Equals(GameStateName.GameState))
            {
                return;
            }

            if (_stateMachine.CurrentState is StateMachine stateMachine)
            {
                _stateMachineMap.TryAdd(stateMachine.Id, stateMachine);
            }

            _rollbackStateStack.Push(eventData);
        }

        private void HandleGoToPreviousStateEvent(IMessage message)
        {
            GoToPreviousState();
        }
    }
}
