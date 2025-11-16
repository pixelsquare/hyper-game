using System;

/// <summary>
/// Wrapper for haptic feedbacks
/// Currently uses NiceVibrations via Feel SDK 2.3
/// </summary>
namespace Kumu.Kulitan.Common
{
    public static class HapticFeedbackHelper
    {
        public static bool IsHapticSupported()
        {
            return false;
        }
    
        public static void SetHapticsActive(bool isActive)
        {
            throw new NotImplementedException();
        }
    
        public static void StopHaptics()
        {
            throw new NotImplementedException();
        }

        public static void PlayTransientHaptic(float intensity, float sharpness)
        {
            throw new NotImplementedException();
        }

        public static void PlayContinuousHaptic(float intensity, float sharpness, float duration)
        {
            throw new NotImplementedException();
        }
        
        #region Presets
        public static void PlaySelectionHaptic()
        {
            throw new NotImplementedException();
        }
        
        public static void PlaySuccessHaptic()
        {
            throw new NotImplementedException();
        }
    
        public static void PlayWarningHaptic()
        {
            throw new NotImplementedException();
        }
        
        public static void PlayFailureHaptic()
        {
            throw new NotImplementedException();
        }
        
        public static void PlayLightImpactHaptic()
        {
            throw new NotImplementedException();
        }
        
        public static void PlayMediumImpactHaptic()
        {
            throw new NotImplementedException();
        }
        
        public static void PlayHeavyImpactHaptic()
        {
            throw new NotImplementedException();
        }
        
        public static void PlayRigidImpactHaptic()
        {
            throw new NotImplementedException();
        }
        
        public static void PlaySoftImpactHaptic()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
