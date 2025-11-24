using UnityEngine;

namespace Santelmo.Rinsurv
{
    public class StateMachineMono : MonoBehaviour
    {
        private IState _currentState;
        private StateMachine _sm;
        private bool _hasStarted;

        public void Initialize(StateMachine stateMachine)
        {
            _sm = stateMachine;
            _currentState = _sm?.CurrentState;
            _hasStarted = false;
        }

        private void Update()
        {
            if (_currentState == null || _sm == null || !_sm._isPlaying)
            {
                return;
            }

            _currentState.OnUpdate();
            _hasStarted = true;
        }

        private void FixedUpdate()
        {
            if (_currentState == null || _sm == null || !_sm._isPlaying)
            {
                return;
            }

            _currentState.OnFixedUpdate();
        }

        private void LateUpdate()
        {
            // `hasStarted` flag makes sure that Update method has been called for 1 frame.
            // For some reason on Unit Testing, Late Update is being called before Update.
            if (_currentState == null || _sm == null || !_sm._isPlaying || !_hasStarted)
            {
                return;
            }

            _currentState.OnLateUpdate();
            _sm.PollStateMachineEndState();
            _sm.PollStateCondition();
        }
    }
}
