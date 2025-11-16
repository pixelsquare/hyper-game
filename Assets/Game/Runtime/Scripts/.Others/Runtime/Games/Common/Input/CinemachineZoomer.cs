using System;
using Cinemachine;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class CinemachineZoomer : MonoBehaviour
    {
        private enum ZoomState { In, Out, None }
        
        [SerializeField] private CinemachineFreeLook freeLook;
        [SerializeField] private AnimationCurve zoomTransition;
        [SerializeField] private float duration;
        [SerializeField] private ZoomState startingState;

        private float current;
        private float inExpectedValue;
        private float outExpectedValue;
        private ZoomState zoomState;

        public void ToggleZoom()
        {
            switch (zoomState)
            {
                case ZoomState.None:
                    zoomState = Math.Abs(current - inExpectedValue) < Mathf.Epsilon ? ZoomState.Out : ZoomState.In;
                    break;
                case ZoomState.In:
                    zoomState = ZoomState.Out;
                    break;
                case ZoomState.Out:
                    zoomState = ZoomState.In;
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        private void UpdateZoomStateValues(float incrementValue)
        {
            current += incrementValue;
            if (current > inExpectedValue && current < outExpectedValue)
            {
                return;
            }
            
            current = Mathf.Clamp(current, inExpectedValue, outExpectedValue);
            zoomState = ZoomState.None;
        }

        private void UpdateCinemachineYAxisValue()
        {
            var progressValue = Mathf.InverseLerp(inExpectedValue, outExpectedValue, current);
            freeLook.m_YAxis.Value = zoomTransition.Evaluate(progressValue);
        }

        private float StateToValue(ZoomState stateToEval)
        {
            return stateToEval == ZoomState.In ? inExpectedValue : outExpectedValue;
        }

        private void Update()
        {
            switch (zoomState)
            {
                case ZoomState.In:
                    UpdateZoomStateValues(-Time.unscaledDeltaTime);
                    UpdateCinemachineYAxisValue();
                    break;
                case ZoomState.Out:
                    UpdateZoomStateValues(Time.unscaledDeltaTime);
                    UpdateCinemachineYAxisValue();
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        private void Awake()
        {
            inExpectedValue = 0f;
            outExpectedValue = duration;
        }

        private void Start()
        {
            zoomState = startingState;
            current = StateToValue(zoomState);
            UpdateCinemachineYAxisValue();
        }
    }
}
